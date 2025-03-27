using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using BackMange.Models;
using System.Net.Http.Headers;
using System.Web;
using System.Text;
using BackMange.DTO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BackMange.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController(GameCaseContext context, INotificationService notificationService, IHttpContextAccessor httpContextAccessor) : ControllerBase
    {
        private GameCaseContext _context = context;
        private INotificationService _notificationService = notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        [HttpGet("LoadNotis/{userId:int}")]
        public async Task<ActionResult> LoadNotis(int userId)
        {
            //Console.WriteLine("===LoadNotis!!!!!===");
            try
            {
                // 只查詢一次
                var unreadCount = await _context.TUserNotifications
                    .Where(n => n.FUserId == userId && !n.FIsRead)
                    .CountAsync();

                // 取得通知資料
                var notifications = await _context.TUserNotifications
                    .Where(u => u.FUserId == userId && u.FCreatedAt != null)
                    .OrderByDescending(u => u.FCreatedAt)
                    .ToListAsync();  

                if (notifications == null || !notifications.Any())
                {
                    return Ok(new { message = "沒有通知" });
                }

                //// 打印 notifications
                //Console.WriteLine("=== notifications 結果 ===");
                //foreach (var noti in notifications)
                //{
                //    Console.WriteLine($"ID: {noti.FNotificationId}, " +
                //                    $"Message: {noti.FMessage}, " +
                //                    $"Type: {noti.FNotifyType}, " +
                //                    $"CreatedAt: {noti.FCreatedAt}");
                //}

                // 使用同一個 unreadCount
                var notis = notifications.Select(u => new NotiObj
                {   
                    NotificationId = u.FNotificationId,
                    UserId = u.FUserId,
                    Content = u.FMessage,
                    Type = u.FNotifyType,
                    CreatedTime = u.FCreatedAt,
                    IsRead = u.FIsRead,
                    TimeAgo = NotiObj.GetTimeAgo(u.FCreatedAt.Value),
                    UnReads = unreadCount,
                    Link = GetLink(u.FMessage,u.FNotifyType, u.FRelatedId)
                }).ToList();

                //// 打印 notis
                //Console.WriteLine("=== notis 結果 ===");
                //foreach (var noti in notis)
                //{
                //    Console.WriteLine($"ID: {noti.NotificationId}, " +
                //                    $"Content: {noti.Content}, " +
                //                    $"Type: {noti.Type}, " +
                //                    $"TimeAgo: {noti.TimeAgo}, "
                //                    +  $"Link: {noti.Link}");
                //}

                return Ok(new { 
                    success = true, 
                    message = "成功載入通知",
                    notifications = notis 
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    success = false, 
                    message = ex.Message,
                    notifications = new List<NotiObj>()
                });
            }
        }

        [HttpPost("TaskNotification")]
        public async Task<ActionResult> TaskNotification([FromBody] NotiObj notification)
        {
            try
            {
                // 記錄收到的請求
                //Console.WriteLine($"收到的通知資料: {JsonSerializer.Serialize(notification)}");
                // 檢查 notification 是否為 null
                if (notification == null) return BadRequest(new { success = false, message = "通知資料不能為空" });
                
                // 檢查是否已發送過通知
                var existingNoti = await _context.TUserNotifications
                    .FirstOrDefaultAsync(n =>
                        n.FUserId == notification.UserId &&
                        n.FSenderId == notification.SenderId &&
                        n.FRelatedId == notification.RelatedId &&
                        n.FNotifyType == "Case" &&
                        n.FMessage.Contains("新的應徵")
                        );
                if (existingNoti == null)
                {
                    // 檢查 _notificationService 是否為 null
                    if (_notificationService == null) return StatusCode(500, new { success = false, message = "通知服務未正確初始化" });
              
                    // 呼叫通知服務
                    var (success, message, result) = await _notificationService.SendNotificationAsync(notification);
                    if (!success) return BadRequest(new { success = false, message = "呼叫通知服務失敗!" });
                   
                    return Ok(new { success = true, message = message, data = result });
                }
                return Ok(new { success = true, message = "已發過" });
            }
            catch (Exception ex)
            {
                // 記錄異常
                Console.WriteLine($"發送通知時發生錯誤: {ex.Message}");
                return StatusCode(500, new { 
                    success = false, 
                    message = "發送通知時發生未預期的錯誤",
                    error = ex.Message 
                });
            }
        }

        [HttpPut("MarkAsRead/{notificationId:int}")]
        public async Task<ActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var notification = await _context.TUserNotifications
                    .FirstOrDefaultAsync(n => n.FNotificationId == notificationId);

                if (notification == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "找不到指定的通知" 
                    });
                }

                notification.FIsRead = true;
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "通知已標記為已讀"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "更新通知狀態時發生錯誤",
                    error = ex.Message 
                });
            }
        }

        [HttpPut("MarkAllAsRead/{userId:int}")]
        public async Task<ActionResult> MarkAllAsRead(int userId)
        {
            try
            {
                var unreadNotifications = await _context.TUserNotifications
                    .Where(n => n.FUserId == userId && !n.FIsRead)
                    .ToListAsync();

                if (!unreadNotifications.Any())
                {
                    return Ok(new { 
                        success = true, 
                        message = "沒有未讀通知" 
                    });
                }
                
                foreach (var notification in unreadNotifications)
                {
                    notification.FIsRead = true;
                }
                
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "所有通知已標記為已讀" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "更新通知狀態時發生錯誤",
                    error = ex.Message 
                });
            }
        }

        // 定義通知類型列舉(用來避免關鍵字衝突)
        public enum NotificationType
        {
            Case,
            Payment,
            Chat,
            System
        }

        private string GetLink(string msg, string notifyType, int? relatedId)
        {
            // 獲取基礎 URL
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";          

            if (!Enum.TryParse<NotificationType>(notifyType, true, out var type))
            {
                return "#";
            }

            switch (type)
            {
                case NotificationType.Case:
                    if (msg.Contains("新的應徵"))
                    {
                        return $"{baseUrl}/FProfileManage/PersonalPage#/Poster";
                    }
                    else if (msg.Contains("已審核通過"))
                    {
                        return $"{baseUrl}/Frontend/missionDetail/{relatedId}";
                    }
                    else if (msg.Contains("案主核准"))
                    {
                        return $"{baseUrl}/Frontend/missionDetail/{relatedId}";
                    }
                    else return "#";
                    
                case NotificationType.Payment:
                    return $"{baseUrl}/FProfileManage/PersonalPage#/Freelancer";
                                                      
                case NotificationType.Chat:
                    return $"{baseUrl}/FProfileManage/PersonalPage#/Message";
                    
                case NotificationType.System:
                    if (msg.Contains("歡迎加入")) return $"{baseUrl}/FProfileManage/PersonalPage#/FreelancerProfile";
                    else return $"#";
                    
                    
                default:
                    return "#";
            }
        }





    }
}
