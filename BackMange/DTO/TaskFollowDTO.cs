using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BackMange.Models;

namespace BackMange.DTO
{
    public class TaskFollowDTO
    {
        public TaskFollowDTO()
        {
            // 無參數建構函式
        }

        public TaskFollowDTO(TTaskFollow follow)
        {
            FFollowID = follow.FfollowId;
            FFollowerID = follow.FfollowerId;
            FTaskID = follow.FtaskId;
            FCreatedAt = follow.FcreatedAt ?? DateTime.Now;

            // 如果有關聯的用戶和任務資料，設置名稱
            if (follow.Ffollower != null)
            {
                FollowerName = follow.Ffollower.FfullName;
            }

            if (follow.Ftask != null)
            {
                TaskTitle = follow.Ftask.Ftitle;
                TaskBudget = follow.Ftask.Fbudget;
                TaskDeadline = follow.Ftask.Fdeadline;
            }
        }

        [DisplayName("追蹤編號")]
        public int FFollowID { get; set; }

        [DisplayName("追蹤者ID")]
        public int FFollowerID { get; set; }

        [DisplayName("任務ID")]
        public int FTaskID { get; set; }

        [DisplayName("追蹤時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}")]
        public DateTime FCreatedAt { get; set; }

        // 導覽屬性
        [DisplayName("追蹤者名稱")]
        public string? FollowerName { get; set; }

        [DisplayName("任務標題")]
        public string? TaskTitle { get; set; }

        [DisplayName("任務預算")]
        public int TaskBudget { get; set; }

        [DisplayName("截止日期")]
        public DateTime TaskDeadline { get; set; }
    }
}

