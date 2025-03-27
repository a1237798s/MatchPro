using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BackMange.Models;

namespace BackMange.DTO
{
    // 使用者資料傳輸物件 (DTO)，用於在前後端之間傳遞使用者資料
    public class CUserDTO
    {
        // 使用者唯一識別碼 (主鍵)
        public int FuserId { get; set; }

        // 使用者全名，必填欄位
        [Required(ErrorMessage = "姓名為必填欄位")]
        [DisplayName("姓名")]
        public string FfullName { get; set; } = null!;

        // 電子郵件，必填且需符合 Email 格式
        [Required(ErrorMessage = "電子郵件為必填欄位")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件格式")]
        [DisplayName("電子郵件")]
        public string Femail { get; set; } = null!;

        // 密碼雜湊值
        [DisplayName("密碼")]
        public string FpasswordHash { get; set; } = null!;

        // 手機號碼，選填
        [DisplayName("手機號碼")]
        public string? FphoneNumber { get; set; }

        // 地址，選填
        [DisplayName("地址")]
        public string? Faddress { get; set; }

        // 生日，使用 DateOnly 型別
        [DisplayName("生日")]
        [DataType(DataType.Date)]
        public DateOnly? Fbirthday { get; set; }

        // 性別（M=男, F=女, O=其他）
        [DisplayName("性別")]
        public string? Fgender { get; set; }

        // 個人頭像圖片的 URL
        [DisplayName("照片")]
        public string? FprofileImageUrl { get; set; }

        // 最後登入時間
        [DisplayName("最後登入時間")]
        public DateTime? FlastLoginTime { get; set; }

        // 電子郵件是否已驗證，預設為 false
        [DisplayName("信箱驗證")]
        public bool? FisEmailVerified
        {
            get => _isEmailVerified ?? false;  // 如果是 null 就返回 false
            set => _isEmailVerified = value;
        }
        private bool? _isEmailVerified;

        // 帳號狀態：0=未啟用, 1=啟用, 2=停用, 3=刪除
        [DisplayName("帳號狀態")]
        public byte? Fstatus { get; set; }

        // 帳號停用原因
        [DisplayName("停用原因")]
        public string? FsuspensionReason { get; set; }

        // 停用到期時間
        [DisplayName("停用結束時間")]
        public DateTime? FsuspensionEndTime { get; set; }

        // 帳號建立時間
        [DisplayName("建立時間")]
        public DateTime? FcreatedAt { get; set; }

        // 最後更新時間
        [DisplayName("更新時間")]
        public DateTime? FupdatedAt { get; set; }

        // Google 登入用的 ID
        [DisplayName("Google ID")]
        public string? FgoogleId { get; set; }

        // 登入類型（Local=一般登入, Google=Google登入）
        [DisplayName("登入類型")]
        public string FloginType { get; set; } = "Local";  // 預設為一般登入

        // 身分證字號
        [DisplayName("身分證字號")]
        public string? FidNumber { get; set; }

        // 公司統一編號
        [DisplayName("統一編號")]
        public string? FcompanyNumber { get; set; }

        // 使用者類型 ID 列表
        [DisplayName("用戶類型")]
        public List<int> FuserTypes { get; set; } = new List<int>();

        // 使用者類型名稱列表
        [DisplayName("用戶類型名稱")]
        public List<string> UserTypeNames { get; set; } = new List<string>();

        // 將數字狀態轉換為可讀文字的屬性
        [DisplayName("狀態")]
        public string StatusText => Fstatus switch
        {
            0 => "未啟用",
            1 => "啟用",
            2 => "停用",
            3 => "刪除",
            _ => "未知"
        };

        // 停權原因的列舉
        public enum SuspensionReason
        {
            [Display(Name = "違反社群規範")]
            ViolateCommunityGuidelines = 1,

            [Display(Name = "發布不當內容")]
            InappropriateContent = 2,

            [Display(Name = "帳號安全疑慮")]
            SecurityConcern = 3,

            [Display(Name = "多次違規")]
            RepeatedViolations = 4,

            [Display(Name = "惡意騷擾他人")]
            Harassment = 5,

            [Display(Name = "其他原因")]
            Other = 6
        }

        // 預設建構函式
        public CUserDTO() { }

        // 從 TUser 模型轉換為 DTO 的建構函式
        public CUserDTO(TUser user)
        {
            // 複製基本資料
            FuserId = user.FuserId;
            FfullName = user.FfullName;
            Femail = user.Femail;
            FpasswordHash = user.FpasswordHash;
            FphoneNumber = user.FphoneNumber;
            Faddress = user.Faddress;
            Fbirthday = user.Fbirthday;
            Fgender = user.Fgender;
            FprofileImageUrl = user.FprofileImageUrl;
            FlastLoginTime = user.FlastLoginTime;
            FisEmailVerified = user.FisEmailVerified ?? false;
            Fstatus = user.Fstatus;
            FsuspensionReason = user.FsuspensionReason;
            FsuspensionEndTime = user.FsuspensionEndTime;
            FcreatedAt = user.FcreatedAt;
            FupdatedAt = user.FupdatedAt;
            FgoogleId = user.FgoogleId;
            FloginType = user.FloginType;
            FidNumber = user.FidNumber;
            FcompanyNumber = user.FcompanyNumber;

            // 轉換使用者類型資料
            FuserTypes = user.FuserTypes?.Select(ut => ut.FuserTypeId).ToList() ?? new List<int>();
            UserTypeNames = user.FuserTypes?.Select(ut => ut.FuserTypeName).ToList() ?? new List<string>();
        }
    }
}