using System.ComponentModel.DataAnnotations;

namespace BackMange.ViewModels
{
    public class LoginViewModel
    {
    [Required(ErrorMessage = "請輸入Email")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "請輸入密碼")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
    // 接收前端傳來的登入資料    
       
    }    
   
}
