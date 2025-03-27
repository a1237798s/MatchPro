namespace BackMange.Models
{
    public interface INotificationService
    {
        Task<(bool success, string message, NotiObj notification)> SendNotificationAsync(NotiObj notification);
    }
}
