using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BackMange.Models;

namespace BackMange.DTO
{
    public class WorkerFollowDTO
    {
        public WorkerFollowDTO()
        {
            // 無參數建構函式
        }

        [DisplayName("追蹤編號")]
        public int FFollowID { get; set; }

        [DisplayName("追蹤者ID")]
        public int FFollowerID { get; set; }

        [DisplayName("工作者ID")]
        public int FWorkerUserID { get; set; }

        [DisplayName("追蹤時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime FCreatedAt { get; set; }

        // 導覽屬性
        [DisplayName("追蹤者名稱")]
        public string? FollowerName { get; set; }

        [DisplayName("工作者名稱")]
        public string? WorkerName { get; set; }
    }
}
