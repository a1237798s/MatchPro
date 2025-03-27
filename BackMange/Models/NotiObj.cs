namespace BackMange.Models
{
    public class NotiObj
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public required string Content { get; set; }
        public int? RelatedId { get; set; }  // 相關案件編號
        public int? SenderId { get; set; }  // 對方用戶ID
        public required string Type { get; set; } 
        public bool? IsRead { get; set; }  
        public int? UnReads { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string? TimeAgo { get; set; }
        public string? Icon { get; set; } 
        public string? Link { get; set; }

        // 加入計算時間差的方法
        public static string GetTimeAgo(DateTime dateTime)
        {

            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "剛剛";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} 分鐘前";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} 小時前";
            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} 天前";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} 個月前";

            return $"{(int)(timeSpan.TotalDays / 365)} 年前";
        }
    }

    // 通知類型
    public enum NotificationType
    {
        Task,  // 案件
        Msg,       // 訊息
        Payment,    // 款項
        System         // 系統通知
    }


}