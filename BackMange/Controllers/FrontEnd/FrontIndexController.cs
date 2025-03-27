using Microsoft.AspNetCore.Mvc;


namespace BackMange.Controllers.FrontEnd
{
    public class FrontIndexController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

       
    }
}
