using System.Net.WebSockets;
using BackMange.Controllers.API;
using BackMange.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace HTML5.Hubs
{
    public class ChatHub : Hub
    {
        private readonly GameCaseContext _context;
        private readonly INotificationService _notificationService;

        public ChatHub(GameCaseContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        private static int  chatId = 0;

        
        //========================================

        public int ChangeChatID(int currentChatID)
        {
            chatId = currentChatID;
            return chatId;
        }
        /// <summary>
        /// 連線事件
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            string posterIdString = Context.GetHttpContext().Request.Query["PosterId"]; // 獲取發案方
            string userIdString = Context.GetHttpContext().Request.Query["UserId"]; // 獲取用戶 ID

            // 直接轉型為 int
            int posterId = Convert.ToInt32(posterIdString);
            int userId = Convert.ToInt32(userIdString);

            // 查詢是否已經存在 A 與 B 的一對一聊天室
            var existingChat = await _context.TChats
                .FirstOrDefaultAsync(c =>
                    c.FPosterId == posterId && c.FWorkerId == userId);
            //  ||  (c.PosterId == otherUserId && c.WorkerId == userId));

            if (existingChat != null)
            {
                // 如果聊天室已存在，則加入該聊天室
                await Groups.AddToGroupAsync(Context.ConnectionId, existingChat.FChatId.ToString());
                await Clients.Group(existingChat.FChatId.ToString()).SendAsync("UpdContent", "用戶 ID: " + Context.ConnectionId + " 重新連接到聊天室: ");
                
                ChangeChatID(existingChat.FChatId);


                Console.WriteLine($"當前chatId 已存在: {chatId}");

            }
            else
            {
                // 如果聊天室不存在，則創建新的聊天室
                var newChat = new TChat
                {

                    FPosterId = posterId,
                    FWorkerId = userId,

                };

                _context.TChats.Add(newChat);
                await _context.SaveChangesAsync();

                var newChatRoom = await _context.TChats
                .FirstOrDefaultAsync(c =>
                    (c.FPosterId == posterId && c.FWorkerId == userId));
                // 將用戶加入新的聊天室
                await Groups.AddToGroupAsync(Context.ConnectionId, newChatRoom.FChatId.ToString());
                await Clients.Group(newChatRoom.FChatId.ToString()).SendAsync("UpdContent", "新連線 ID: " + Context.ConnectionId + " 進入聊天室: ");
                ChangeChatID(newChatRoom.FChatId);


                Console.WriteLine($"當前chatId new : {chatId}");

            }

            await base.OnConnectedAsync();
        }
        //public override async Task OnConnectedAsync()
        //{
        //    await Clients.All.SendAsync("UpdContent", "新連線 ID: " + Context.ConnectionId);
        //    await base.OnConnectedAsync();
        //}

        /// <summary>
        /// 離線事件
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // 使用當前的 chatId 來發送離線訊息到特定群組
            await Clients.Group(chatId.ToString()).SendAsync("UpdContent", "已離線 ID: " + Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }

        /// <summary>
        /// 傳遞訊息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public async Task SendMessage(int user, string messagesd)
        {
            Console.WriteLine($"SendMessage ! : {user} :  {messagesd}");
            //for 前端顯示
            var msg = new
            {
                User = user,
                Message = messagesd,
                Timestamp = DateTime.Now.ToString("HH:mm")
            };
            
            // 發送訊息到特定群組
            await Clients.Group(chatId.ToString()).SendAsync("UpdContent", msg);
            
            //Console.WriteLine($"chatId send: {chatId} :  {messagesd}");

            //var currentChat = await _context.TChats
            //    .FirstOrDefaultAsync(c =>
            //        c.PosterId == PposterId && c.WorkerId == PuserId);

            //for SQL
            var chatMessage = new TMessagesHistory
            {
                FSenderId = user,
                FMessageText = messagesd,
                FChatId = chatId,
                FSentAt = DateTime.Now
            };

            _context.TMessagesHistories.Add(chatMessage);
            await _context.SaveChangesAsync();

            //此處為發送通知
            #region === 發送通知 ===
            var chat = await _context.TChats
                .FirstOrDefaultAsync(c => c.FChatId == chatId);

            if (chat != null)
            {
                try 
                {
                    var otherId = user == chat.FPosterId ? chat.FWorkerId : chat.FPosterId;
                    
                    // 先刪除舊的聊天通知
                    var oldNotifications = await _context.TUserNotifications
                        .Where(n => n.FUserId == otherId 
                               && n.FNotifyType == "Chat" 
                               && n.FRelatedId == chatId
                               && n.FSenderId == user)
                        .ToListAsync();

                    if (oldNotifications.Any())
                    {
                        _context.TUserNotifications.RemoveRange(oldNotifications);
                        await _context.SaveChangesAsync();
                    }
                    
                    // 判斷角色
                    string roleText = user == chat.FPosterId ? "案主" : "接案者";
                    string otherUserName;

                    // 根據角色查詢對應資料表
                    if (roleText == "接案者")
                    {
                        var theWorker = await _context.TWorkers
                            .Where(w => w.FuserId == user)
                            .Select(w => new { Name = w.FcodeName })
                            .FirstOrDefaultAsync();
                            
                        if (theWorker == null)
                        {
                            Console.WriteLine($"找不到接案者 ID: {otherId}");
                            return;
                        }
                        otherUserName = theWorker.Name;
                    }
                    else // 案主
                    {
                        var thePoster = await _context.TUsers
                            .Where(u => u.FuserId == user)
                            .Select(u => new { Name = u.FfullName })
                            .FirstOrDefaultAsync();
                            
                        if (thePoster == null)
                        {
                            Console.WriteLine($"找不到案主 ID: {otherId}");
                            return;
                        }
                        otherUserName = thePoster.Name;
                    }

                    string notificationContent = $"{roleText} {otherUserName} 發送一則訊息給您。";

                    var notification = new NotiObj
                    {
                        UserId = otherId,
                        Content = notificationContent,
                        Type = "Chat",
                        RelatedId = chatId,
                        SenderId = user,
                    };

                    var (success, message, _) = await _notificationService.SendNotificationAsync(notification);
                    if (!success) 
                    {
                        Console.WriteLine($"通知發送失敗：{message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"處理通知時發生錯誤：{ex.Message}");
                }
            }
          
            #endregion === 發送通知 END===

        }

    }
}
