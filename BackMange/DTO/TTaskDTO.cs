using BackMange.Models;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BackMange.DTO
{
    public class TTaskDTO
    {

        [DisplayName("任務ID")]
        public int FtaskId { get; set; }

        [DisplayName("發案人ID")]
        public int FposterId { get; set; }

        [DisplayName("任務類別ID")]
        public int FcategoryId { get; set; }  // **關聯 tCategory**

        [Required(ErrorMessage = "為必填欄位")]
        [DisplayName("任務標題")]
        public string Ftitle { get; set; }

        [Required(ErrorMessage = "為必填欄位")]
        [DisplayName("任務內容")]
        public string Fdescription { get; set; }
        [Required(ErrorMessage = "為必填欄位")]


        [DisplayName("預算")]
        public int Fbudget { get; set; }

        
        [Required(ErrorMessage = "為必填欄位")]
        [DisplayName("地點")]
        public string Flocation { get; set; }
        [Required(ErrorMessage = "為必填欄位")]
        [DisplayName("詳細地址")]
        public string FlocationDetail { get; set; }
        [Required(ErrorMessage = "為必填欄位")]
        [DisplayName("聯絡人姓名")]
        public string Fmember { get; set; }
        [Required(ErrorMessage = "為必填欄位")]
        [DisplayName("聯絡人電話")]
        public string Fphone { get; set; }
        [Required(ErrorMessage = "為必填欄位")]
        [DisplayName("聯絡人email")]
        public string Femail { get; set; }

        [DisplayName("任務狀態")]
        public string Fstatus { get; set; }

        [DisplayName("截止日期")]
        public DateTime Fdeadline { get; set; }

        [DisplayName("建立時間")]
        public DateTime? FcreatedAt { get; set; }

        [DisplayName("更新時間")]
        public DateTime? FupdatedAt { get; set; }

        [DisplayName("任務照片 (路徑)")]
        public string? FtimagePath { get; set; }

        [DisplayName("任務照片 (多張上傳)")]
        public List<IFormFile>? FtimageFiles { get; set; }
        [DisplayName("追蹤數")]
    
        public int FollowCount { get; set; }



        // **新增會員相關資訊**
        public TUser? User { get; set; }  // 會員資料
        public TPoster? Poster { get; set; }  // 發案者資料
        public TCategory? Category { get; set; }  // 發案者資料

        [DisplayName("發案公司名稱")]
        public string? FCompanyName { get; set; }

        [DisplayName("發案人姓名")]
        public string? FFullName { get; set; }

        [DisplayName("發案人頭像")]
        public string? FProfileImageUrl { get; set; }

        [DisplayName("案件類型")]
        public string? JobName { get; set; }
        // 🔥 新增這行：用來存放目前登入者的 ID
        public int CurrentUserId { get; set; }
        public int? FAcceptedUserId { get; set; } // 接案者 ID（nullable，避免出錯）
        public TTaskDTO() { }

        public int TransactionId { get; set; }

        public int? Rating { get; set; }

        public string? Review { get; set; }
        public int RemainingDays { get; set; }
        public TTaskDTO(TTask td)
        {
            FtaskId = td.FtaskId;
            FposterId = td.FposterId;
            FcategoryId = td.FcategoryId;
            Ftitle = td.Ftitle;
            Fdescription = td.Fdescription;
            Fbudget = td.Fbudget;
            Flocation = td.Flocation;
            FlocationDetail = td.FlocationDetail;
            Fmember = td.Fmember;
            Fphone = td.Fphone;
            Femail = td.Femail;
            Fstatus = td.Fstatus;
            Fdeadline = td.Fdeadline;
            FcreatedAt = td.FcreatedAt;
            FupdatedAt = td.FupdatedAt;
            FtimagePath = td.Ftimage;
            FollowCount = 0;  // 預設追蹤數為 0

        }

        public int FconfirmReplyId { get; set; }

        public int FworkerId { get; set; }

        public string FconfirmationType { get; set; }

        public string FconfirmationStatus { get; set; }

        public string Fremarks { get; set; }
    }
}
