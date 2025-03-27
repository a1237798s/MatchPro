using Microsoft.AspNetCore.Mvc;

namespace BackMange.Controllers.FrontEnd
{
    public class FProfileManageController : Controller
    {
        public IActionResult PersonalPage() 
        {
            var UserID = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(UserID))
            {
                TempData["AlertMessage"] = "請先登入再進入該頁面！";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }        
      
    }
}
