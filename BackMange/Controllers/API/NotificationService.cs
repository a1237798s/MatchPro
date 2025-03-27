using BackMange.Models;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Controllers.API
{
    public class NotificationService : INotificationService
    {
        private readonly GameCaseContext _context;

        public NotificationService(GameCaseContext context)
        {
            _context = context;
        }

        public async Task<(bool success, string message, NotiObj notification)> SendNotificationAsync(NotiObj notification)
        {
            try
            {
                if (notification.UserId <= 0)
                {
                    return (false, "無效的接收者ID", null);
                }

                var newNotification = new TUserNotification
                {
                    FUserId = notification.UserId,
                    FMessage = notification.Content,
                    FRelatedId = notification.RelatedId,
                    FSenderId = notification.SenderId,
                    FNotifyType = notification.Type ?? "System",
                    FIsRead = false,
                    FCreatedAt = DateTime.Now,
                };

                await _context.TUserNotifications.AddAsync(newNotification);
                await _context.SaveChangesAsync();

                var result = new NotiObj
                {
                    NotificationId = newNotification.FNotificationId,
                    UserId = newNotification.FUserId,
                    Content = newNotification.FMessage,
                    Type = newNotification.FNotifyType,
                    IsRead = newNotification.FIsRead,
                    CreatedTime = newNotification.FCreatedAt,
                    RelatedId = newNotification.FRelatedId,
                    SenderId = newNotification.FSenderId,
                    UnReads = await _context.TUserNotifications.CountAsync(n => n.FUserId == notification.UserId && !n.FIsRead)
                };

                return (true, "通知發送成功", result);
            }
            catch (Exception ex)
            {
                return (false, $"發送通知失敗: {ex.Message}", null);
            }
        }
    }
}
