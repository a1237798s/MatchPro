using BackMange.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Controllers.API
{
    [Route("api/ShowMsg")]
    [ApiController]
    public class ShowMsgController(GameCaseContext context, INotificationService notificationService, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        private INotificationService _notificationService = notificationService;
        private  GameCaseContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public class MessageDetail
        {
            public int ChatId { get; set; }
            public string Content { get; set; }
            public string Time { get; set; }
            public string Type { get; set; }
            public int SenderId { get; set; }
            public bool IsRead { get; set; }
        }

        public class MessageResponse
        {
            public List<ChatMessageDto> Messages { get; internal set; }
            public int TotalCount { get; internal set; }
        }

        public class ChatMessageDto
        {
            public int ChatId { get; set; }
            public string UserName { get; set; }
            public string Avatar { get; set; }
            public string LastMessage { get; set; }
            public string Time { get; set; }
            public int Unread { get; set; }
            public string Status { get; set; }
            public List<MessageDetail> AllMessages { get; set; }
        }

        public class MarkAsReadRequest
        {
            public int UserId { get; set; }
        }

        //POST: api/ShowMsg/ListMsg
        [HttpPost("ListMsg")]
        public async Task<IActionResult> ListMsg([FromBody] MsgObj data)
        {
            //Console.WriteLine("ListMsg====");
            try
            {
                List<int> chatRoomIds = new List<int>();

                if (data.msgType == "Post")
                {
                    chatRoomIds = _context.TChats
                                .Where(c => c.FPosterId == data.userId)
                                .Select(c => c.FChatId)
                                .ToList();
                }
                else if (data.msgType == "Work")
                {
                    if (data.posterId == null){
                        chatRoomIds = _context.TChats
                                .Where(c => c.FWorkerId == data.userId)
                                .Select(c => c.FChatId)
                                .ToList();
                        Console.WriteLine("Work 全部型觸發!!!");
                    }
                    else{
                        chatRoomIds = _context.TChats
                                .Where(c => c.FWorkerId == data.userId && c.FPosterId == data.posterId)
                                .Select(c => c.FChatId)
                                .ToList();
                        Console.WriteLine("Work 單一型觸發!!!", chatRoomIds.Count);

                    }
                }

                var chatMessages = new List<ChatMessageDto>();

                foreach (var chatId in chatRoomIds)
                {
                    // 獲取所有訊息
                    var allMessages = _context.TMessagesHistories
                        .Where(m => m.FChatId == chatId)
                        .Select(m => new MessageDetail
                        {
                            ChatId = m.FChatId,
                            Content = m.FMessageText,
                            Time = m.FSentAt.ToString("o"),
                            Type = m.FSenderId == data.userId ? "sent" : "received",
                            SenderId = m.FSenderId,
                            IsRead = m.FIsRead
                        })
                        .ToList();

                    // 打印所有訊息內容
                    Console.WriteLine($"聊天室 ID: {chatId} 的所有訊息：");
                    foreach (var message in allMessages)
                    {
                        Console.WriteLine($"訊息內容: {message.Content}");
                        Console.WriteLine($"發送時間: {message.Time}");
                        Console.WriteLine($"訊息類型: {message.Type}");
                        Console.WriteLine($"發送者ID: {message.SenderId}");
                        Console.WriteLine($"是否已讀: {message.IsRead}");
                        Console.WriteLine("------------------------");
                    }

                    // 檢查最後一則訊息
                    var lastMessage = allMessages.OrderByDescending(m => m.Time).FirstOrDefault();
                    if (lastMessage == null || 
                        string.IsNullOrEmpty(lastMessage.Content) || 
                        lastMessage.Content == "無訊息")
                    {
                        continue; // 跳過無效訊息
                    }

                    var request = _httpContextAccessor.HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    string imgUrl = baseUrl + "/uploads/Profile/";


                    // 獲取聊天室資訊
                    var chatInfo = _context.TChats
                        .Where(c => c.FChatId == chatId)
                        .Select(c => new
                        {
                            UserName = c.FPosterId == data.userId ? c.FWorker.FfullName : c.FPoster.FfullName,
                            Avatar = c.FPosterId == data.userId
                                ? imgUrl + _context.TUsers.FirstOrDefault(u => u.FuserId == c.FWorkerId).FprofileImageUrl
                                : imgUrl + _context.TUsers.FirstOrDefault(u => u.FuserId == c.FPosterId).FprofileImageUrl,

                            IsRead = c.TMessagesHistories.Any(m => m.FIsRead)
                        })
                        .FirstOrDefault();

                    var otherUserName = chatInfo?.UserName ?? "未知用戶";
                    var avatar = chatInfo?.Avatar ?? "default-avatar.png";
                    var unreadCount = allMessages.Count(m => m.SenderId != data.userId && !m.IsRead);

                    chatMessages.Add(new ChatMessageDto
                    {
                        ChatId = chatId,
                        UserName = otherUserName,
                        Avatar = avatar,
                        LastMessage = lastMessage.Content,
                        Time = lastMessage.Time,
                        Unread = unreadCount,
                        Status = chatInfo?.IsRead == true ? "已回覆" : "待回覆",
                        AllMessages = allMessages
                    });
                }

                var messageResponse = new MessageResponse
                {
                    Messages = chatMessages,
                    TotalCount = chatMessages.Count
                };

                return Ok(messageResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Messages = ex.Message });
            }
        }

        [HttpPut("MarkAsRead/{chatId}")]
        public async Task<IActionResult> MarkAsRead(int chatId, [FromBody] MarkAsReadRequest request)
        {
            try
            {
                // 只標記對方發送的未讀訊息
                var messages = await _context.TMessagesHistories
                    .Where(m => m.FChatId == chatId 
                            && !m.FIsRead 
                            && m.FSenderId != request.UserId) // 確保只標記對方發送的訊息
                    .ToListAsync();

                foreach (var message in messages)
                {
                    message.FIsRead = true;
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
