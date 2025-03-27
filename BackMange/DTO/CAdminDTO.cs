using BackMange.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackMange.DTO
{
    public class CAdminDTO
    {
        public int FadminId { get; set; }

        [Required(ErrorMessage = "管理員編號為必填欄位")]
        [DisplayName("管理員編號")]
        public string FadminNo { get; set; } = null!;

        [Required(ErrorMessage = "姓名為必填欄位")]
        [DisplayName("姓名")]
        public string FfullName { get; set; } = null!;

        [Required(ErrorMessage = "電子郵件為必填欄位")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件格式")]
        [DisplayName("電子郵件")]
        public string Femail { get; set; } = null!;

        [DisplayName("密碼")]
        public string? FadmPassword { get; set; } // 可空
        [DisplayName("確認密碼")]
        [Compare("FadmPassword", ErrorMessage = "密碼與確認密碼不相符")]
        public string? ConfirmPassword { get; set; }  // 新增這個屬性

        [DisplayName("手機號碼")]
        public string? FmobilePhone { get; set; }

        [Required(ErrorMessage = "管理員等級為必填欄位")]
        [DisplayName("管理員等級")]
        public int FadminLevel { get; set; }

        [Required(ErrorMessage = "狀態為必填欄位")]
        [DisplayName("帳號狀態")]
        public byte FstatusId { get; set; }

        [DisplayName("建立時間")]
        public DateTime? FcreatedAt { get; set; }

        [DisplayName("更新時間")]
        public DateTime? FupdatedAt { get; set; }

        public virtual TAdminStatus? Fstatus { get; set; } // 設為可空，避免沒有關聯的時候出錯

        public string FstatusName { get; set; } = "未知"; // 預設為"未知"，避免null錯誤

        public CAdminDTO() { }

        public CAdminDTO(TAdmin admin)
        {
            FadminId = admin.FadminId;
            FadminNo = admin.FadminNo;
            FfullName = admin.FfullName;
            Femail = admin.Femail;
            FadmPassword = admin.FadmPassword;
            FmobilePhone = admin.FmobilePhone;
            FadminLevel = admin.FadminLevel;
            FstatusId = admin.FstatusId;
            FcreatedAt = admin.FcreatedAt;
            FupdatedAt = admin.FupdatedAt;

            // 確保 FstatusName 不為 null
            FstatusName = admin.Fstatus?.FstatusName ?? "未知";
        }
    }
}
