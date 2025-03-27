using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using BackMange.Models;
using System.Net.Http.Headers;
using System.Web;
using System.Text;
using BackMange.DTO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static NuGet.Packaging.PackagingConstants;

namespace BackMange.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcpayController(GameCaseContext context) : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly GameCaseContext _context = context;
        //step4 : 新增訂單
        [HttpPost("AddOrders")]
        //[Route("AddOrders")]
        public string AddOrders(get_localStorage json)
        {
            Console.WriteLine("AddOrders");
            string num = "0";
            try {
                var Orders = new TEcpayOrder
                {
                    MemberId = json.MerchantID,
                    MerchantTradeNo = json.MerchantTradeNo,
                    RtnCode = 0, //未付款
                    RtnMsg = "訂單成功尚未付款",
                    TradeNo = json.MerchantID.ToString(),
                    TradeAmt = json.TotalAmount,
                    PaymentDate = Convert.ToDateTime(json.MerchantTradeDate),
                    PaymentType = json.PaymentType,
                    PaymentTypeChargeFee = "0",
                    TradeDate = json.MerchantTradeDate,
                    SimulatePaid = 0
                };
                _context.TEcpayOrders.Add(Orders);
                _context.SaveChanges();
                Console.WriteLine("訂單更新至資料庫");
                num="OK";
            } catch (Exception ex)
            {
                num = ex.ToString();
                Console.WriteLine("訂單新增失敗");
                
            }
            return num;


        }

        [HttpPost("CreatePayment")]
        public async Task<ActionResult> CreatePayment([FromBody] EcpayPaymentDTO paymentData)
        {
            var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            var website = $"https://localhost:7253";
            var workerId = _context.Ttransactions
                        .Where(c => c.TaskId == paymentData.TaskId)
                        .Select(c => c.WorkUserId)
                         .FirstOrDefault();
            var order = new Dictionary<string, string>
        {
            // 固定參數
            { "MerchantTradeNo",  orderId},
            { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
            { "MerchantID",  "2000132"},
            { "PaymentType",  "aio"},
            { "ChoosePayment",  "ALL"},
            { "EncryptType",  "1"},
            
            // 從前端傳入的參數
            { "TotalAmount",  paymentData.TotalAmount.ToString()},
            { "TradeDesc",  paymentData.TradeDesc ?? "無"},
            { "ItemName",  paymentData.ItemName ?? "測試商品"},
            { "CustomField1",  paymentData.CustomField1 ?? "(案件名稱)"}, //案件名稱
            { "CustomField2",  paymentData.TaskId.ToString() ?? "(案件Id)"}, //TaskId
            { "CustomField3", paymentData.CustomField3.ToString() ?? "無"}, //PosterId
            { "CustomField4", workerId.ToString() ?? "無"}, //WorkerId
            
            // URL 相關參數
            { "ReturnURL",  $"{website}/api/Ecpay/AddPayInfo"},
            { "OrderResultURL", $"{website}/FEcpay/PayInfo/{orderId}"},
            { "PaymentInfoURL",  $"{website}/api/Ecpay/AddAccountInfo"},
            { "ClientRedirectURL",  $"{website}/FEcpay/AccountInfo/{orderId}"},
            
            // 其他固定參數
            { "ExpireDate",  "3"},
            { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},
        };

            order["CheckMacValue"] = GetCheckMacValue(order);
            // 先儲存訂單資料
            try 
            {
                var Orders = new TEcpayOrder
                {
                    MerchantTradeNo = order["MerchantTradeNo"], //交易編號
                    MemberId = order["MerchantID"], //UserId對應的email
                    RtnCode = 0, //未付款
                    RtnMsg = "訂單成功尚未付款",
                    TradeNo = order["MerchantID"],  //特店編號， 2000132 測試綠界編號
                    TradeAmt = int.Parse(order["TotalAmount"]),
                    PaymentDate = Convert.ToDateTime(order["MerchantTradeDate"]),
                    PaymentType = order["PaymentType"],
                    PaymentTypeChargeFee = "0",
                    TradeDate = order["MerchantTradeDate"],
                    SimulatePaid = 0,
                    FtaskId = int.Parse(order["CustomField2"]),
                    FposterId = int.Parse(order["CustomField3"]),
                    FworkerId = int.Parse(order["CustomField4"]),

                };

                _context.TEcpayOrders.Add(Orders);
               

                _context.SaveChanges();

                // 生成付款表單 HTML
                var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>處理中...</title>
                </head>
                <body>
                    <form id='ecpayForm' method='POST' action='https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5'>
                        {string.Join("", order.Select(p => $"<input type='hidden' name='{p.Key}' value='{p.Value}' />"))}
                    </form>
                    <script>
                        // 直接提交表單到綠界
                        document.getElementById('ecpayForm').submit();
                    </script>
                </body>
                </html>";

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                // 記錄錯誤
                Console.WriteLine($"訂單建立失敗: {ex.Message}");
                return BadRequest(new { message = "訂單建立失敗，請稍後再試" });
            }
        }

        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";
            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }

        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            SHA256 sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }




        // HomeController->Index->ReturnURL所設定的
        [HttpPost]
        [Route("AddPayInfo")]
        public ActionResult<HttpResponseMessage> AddPayInfo([FromBody] JObject info)
        {
            Console.WriteLine("AddPayInfo");
            try
            {
                // 檢查請求的內容類型
                var contentType = Request.ContentType;
                Console.WriteLine("請求的內容類型：", contentType);

                // 檢查接收到的資料
                Console.WriteLine("接收到的資料：", info.ToString());

                _cache.Set(info.Value<string>("MerchantTradeNo"), info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                Console.WriteLine("錯誤：", e.Message);
                return ResponseError();
            }
        }
        // HomeController->Index->PaymentInfoURL所設定的
        [HttpPost]
        [Route("AddAccountInfo")]
        public ActionResult<HttpResponseMessage> AddAccountInfo(JObject info)
        {
            Console.WriteLine("AddAccountInfo");
            try
            {
                _cache.Set(info.Value<string>("MerchantTradeNo"), info, TimeSpan.FromMinutes(60));
                return ResponseOK();
            }
            catch (Exception e)
            {
                return ResponseError();
            }
        }
        private HttpResponseMessage ResponseError()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("0|Error");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
        private HttpResponseMessage ResponseOK()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("1|OK");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
