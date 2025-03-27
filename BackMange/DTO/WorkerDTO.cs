    using BackMange.Models;
using System.ComponentModel;

namespace BackMange.DTO
{
    public class WorkerDTO
    {
        [DisplayName("接案者編號")]
        public int FUserId { get; set; }

        [DisplayName("暱稱")]
        public string FCodeName { get; set; }

        [DisplayName("技能")]
        public string FSkills { get; set; }

        [DisplayName("年資")]
        public int? FExperienceYears { get; set; }

        [DisplayName("個人簡介")]
        public string FProfileDescription { get; set; }
        public string ShortDescription => GetShortDescription(FProfileDescription, 15);

        private static string GetShortDescription(string description, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(description))
                return "（無簡介）";

            return description.Length > maxLength
                ? description.Substring(0, maxLength) + "..."
                : description;
        }
        [DisplayName("已完成案件數量")]
        public int? FCompletedTasksCount { get; set; }

        [DisplayName("評分")]
        public double? FRating { get; set; }

        [DisplayName("驗證")]
        public bool? FIsVerified { get; set; }

        [DisplayName("啟用狀態")]
        public bool? FIsDeleted { get; set; }

        [DisplayName("建立時間")]
        public DateTime? FCreatedAt { get; set; }

        [DisplayName("更新時間")]
        public DateTime? FUpdatedAt { get; set; }

        [DisplayName("個人連結")]
        public string? FWebsiteURL { get; set; }

        [DisplayName("關於我")]
        public string? FAboutMe { get; set; }
        [DisplayName("服務時間")]
        public string? FServiceAvailability { get; set; }
        public List<WorkerTransactionDTO> Transactions { get; set; }





        public string ProfileImagePath { get; set; }
        public List<string> BackgroundImagePaths { get; set; }  //2025/2/5 新增這行

        public int FollowCount { get; set; }  //追蹤數量

        public WorkerDTO(TWorker worker, string profileImagePath, List<string> backgroundImagePaths) //2025/2/5 新增這行

        {
            if (worker == null)
                throw new ArgumentNullException(nameof(worker));

            FUserId = worker.FuserId;
            FCodeName = worker.FcodeName;
            FSkills = worker.Fskills;
            FExperienceYears = worker.FexperienceYears;
            FProfileDescription = worker.FprofileDescription;
            FCompletedTasksCount = worker.FcompletedTasksCount;
            FRating = worker.Frating;
            FIsVerified = worker.FisVerified;
            FIsDeleted = worker.FisDeleted;
            FWebsiteURL = worker.FwebsiteUrl;
            FAboutMe = worker.FaboutMe;
            FServiceAvailability = worker.FserviceAvailability;
            FCreatedAt = worker.FcreatedAt;
            FUpdatedAt = worker.FupdatedAt;
            ProfileImagePath = profileImagePath;
            BackgroundImagePaths = backgroundImagePaths; //2025/2/5 新增這行

            //加入明細
            Transactions = worker.Fuser?.TtransactionWorkUsers?
    .Select(t => new WorkerTransactionDTO
    {
        TransactionId = t.TransactionId,
        TaskTitle = t.Task?.Ftitle,
        Amount = t.Amount,
        Status = t.Status,
        StartTime = t.StartTime,
        FinishTime = t.FinishTime,
        Rating = t.Rating,
        Review = t.Review
    }).ToList() ?? new List<WorkerTransactionDTO>();

        }

        public WorkerDTO()
        {
            // 初始化必要的屬性
            FCodeName = string.Empty;
            FSkills = string.Empty;
            ProfileImagePath = "imgs/default-avatar.jpg";
            BackgroundImagePaths = new List<string>();
            Transactions = new List<WorkerTransactionDTO>();
        }
    }

}
