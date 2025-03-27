using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using BackMange.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Controllers.FrontEnd
{
    public class FEcpayController : Controller
    {
        private readonly GameCaseContext _context;
        private readonly INotificationService _notificationService;
        public FEcpayController(GameCaseContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        /// step5 : 取得付款資訊，更新資料庫 OrderResultURL
        [HttpPost] //完成付款後才觸發
        public async Task<ActionResult<Dictionary<string, string>>> PayInfo(IFormCollection id)
        {
            Console.WriteLine("PayInfo被調用");
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data[key] = id[key];
            }

            //Console.WriteLine("=== 以下為訂單資訊 ===");
            //foreach (var item in data)
            //{
            //    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
            //}
            //Console.WriteLine("=== 以上為訂單資訊 ===");

            var Orders = _context.TEcpayOrders.FirstOrDefault(m => m.MerchantTradeNo == id["MerchantTradeNo"].ToString());
            Orders.RtnCode = int.Parse(id["RtnCode"]);
            if (id["RtnMsg"] == "Succeeded") Orders.RtnMsg = "已付款";
            Orders.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
            Orders.SimulatePaid = int.Parse(id["SimulatePaid"]);


            // 更新任務狀態為「已完成」
            var task =  _context.TTasks.FirstOrDefault(m => m.FtaskId ==Orders.FtaskId);
            task.Fstatus = "已完成";
            var transaction = _context.Ttransactions.FirstOrDefault(m => m.TaskId == Orders.FtaskId);
            transaction.Status = "已完成";
            // 檢查是否已發送過付款通知
            var existingNotification = await _context.TUserNotifications
                .FirstOrDefaultAsync(n => 
                    n.FUserId == transaction.WorkUserId && 
                    n.FRelatedId == task.FtaskId && 
                    n.FNotifyType == "Payment" &&
                    n.FMessage.Contains("款項已經到帳"));


            _context.TTasks.Update(task);
            _context.SaveChanges();

            // 只有在沒有發送過通知時才發送
            if (existingNotification == null) 
            {
                // 發送通知給接案者
                var notification = new NotiObj
                {
                    UserId = transaction.WorkUserId,  // 接案者ID
                    Content = $"案件「{task.Ftitle}」的款項已經到帳，請查收",
                    Type = "Payment",              // 使用 Payment 類型
                    RelatedId = task.FtaskId,           // 相關案件ID
                    SenderId = task.FposterId,     // 案主ID
                    Icon = "bi-cash-coin",        // 使用金錢相關的圖示
                    Link = $"/Transaction/Detail/{transaction.TransactionId}"  // 交易詳情頁面連結
                };

                var (success, message, _) = await _notificationService.SendNotificationAsync(notification);

                if (!success) Console.WriteLine($"狀態更新成功，但通知發送失敗：{message}");
                else Console.WriteLine("付款完成，已通知接案者款項已到帳");
            }

                //return View("EcpayView", data); // 原版
                return View("EcpayView_Evie", data); // 客製化
            
        }

        /// step5 : 取得虛擬帳號 資訊  ClientRedirectURL
        [HttpPost]
        public ActionResult AccountInfo(IFormCollection id)
        {
            Console.WriteLine("AccountInfo");
            var data = new Dictionary<string, string>();
            foreach (string key in id.Keys)
            {
                data.Add(key, id[key]);
            }
            
            var Orders = _context.TEcpayOrders.ToList().Where(m => m.MerchantTradeNo == id["MerchantTradeNo"]).FirstOrDefault();
            Orders.RtnCode = int.Parse(id["RtnCode"]);
            if (id["RtnMsg"] == "Succeeded") Orders.RtnMsg = "已付款";
            Orders.PaymentDate = Convert.ToDateTime(id["PaymentDate"]);
            Orders.SimulatePaid = int.Parse(id["SimulatePaid"]);
            _context.SaveChanges();
            return View("EcpayView", data);
        }


    }
}
