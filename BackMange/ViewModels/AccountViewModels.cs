using System;
using System.ComponentModel.DataAnnotations;

namespace BackMange.ViewModels
{
    public class AccountViewModels
    {
        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "請輸入有效的Email格式")]
        [Display(Name = "電子郵件")]
        public string Femail { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [Display(Name = "密碼")]
        public string FpasswordHash { get; set; }

        [Display(Name = "Google帳號")]
        public string? FgoogleId { get; set; }

        [Display(Name = "登入類型")]
        public string FloginType { get; set; } = "Local";
    }

    public class LoginResultViewModel
    {
        public string? ErrMsg { get; set; }
        public string? ResultMsg { get; set; }
        public bool? NeedVerify { get; set; }
        public string? LoginType { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "請輸入姓名")]
        [Display(Name = "姓名")]
        public string FfullName { get; set; }

        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "請輸入有效的Email格式")]
        [Display(Name = "電子郵件")]
        public string Femail { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{6,}$",
        ErrorMessage = "密碼必須包含大小寫字母和數字，且至少6個字元")]
        [Display(Name = "密碼")]
        public string FpasswordHash { get; set; }

        [Required(ErrorMessage = "請輸入確認密碼")]
        [Compare("FpasswordHash", ErrorMessage = "密碼與確認密碼不符")]
        [Display(Name = "確認密碼")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "電話號碼")]
        public string? FphoneNumber { get; set; }

        [Display(Name = "地址")]
        public string? Faddress { get; set; }

        [Display(Name = "生日")]
        public DateTime? Fbirthday { get; set; }

        [Display(Name = "性別")]
        public string? Fgender { get; set; }

        [RegularExpression(@"^[A-Z][12]\d{8}$",
            ErrorMessage = "身分證字號格式不正確")]
        [Display(Name = "身分證字號")]
        public string? FidNumber { get; set; }

        [RegularExpression(@"^\d{8}$",
            ErrorMessage = "統一編號格式不正確")]
        [Display(Name = "統一編號")]
        public string? FcompanyNumber { get; set; }

        [Display(Name = "Google帳號")]
        public string? FgoogleId { get; set; }

        [Display(Name = "登入類型")]
        public string FloginType { get; set; } = "Local";

        public bool ValidateIdNumberAndCompanyNumber()
        {
            return (string.IsNullOrEmpty(FidNumber) && !string.IsNullOrEmpty(FcompanyNumber)) ||
                   (!string.IsNullOrEmpty(FidNumber) && string.IsNullOrEmpty(FcompanyNumber));
        }
    }

    public class RegisterResultViewModel
    {
        public string? ErrMsg { get; set; }
        public string? ResultMsg { get; set; }
    }


    public class CheckRegistrationRequest
    {
        public string Femail { get; set; }
        public string FphoneNumber { get; set; }
        public string FidNumber { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
}