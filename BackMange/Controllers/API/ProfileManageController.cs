using BackMange.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using BackMange.DTO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis.Scripting;
using BackMange.ViewModels;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net.NetworkInformation;
using Humanizer;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Data.Common;
using System.Security.Principal;

namespace BackMange.Controllers.API
{
    // 定義請求類別
    public class EmailRequest
    {
        public string Email { get; set; }
    }

    public class VerificationRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }

    public class ProfileManageController : Controller
    {
        private readonly GameCaseContext _context;
        private readonly VerificationService _verificationService;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        public ProfileManageController(GameCaseContext context, VerificationService verificationService, EmailService emailService, IWebHostEnvironment env)
        {
            _context = context;
            _verificationService = verificationService;
            _emailService = emailService;
            _env = env;
        }

        // Email 格式驗證方法
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, pattern);
        }

        // 發送驗證碼
        [HttpPost]
        public async Task<IActionResult> SendVerification([FromBody] EmailRequest request)
        {
            // 檢查 Email 是否為空
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { Message = "Email 不能為空" });
            }

            // 驗證 Email 格式
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new { Message = "Email 格式不正確，請輸入有效的 Email" });
            }

            // 檢查 Email 是否已存在於資料庫
            bool emailExists = _context.TUsers.Any(u => u.Femail == request.Email);

            if (emailExists)
            {
                return BadRequest(new { Message = "該 Email 已被使用，請使用其他 Email" });
            }

            var code = _verificationService.GenerateVerificationCode(request.Email);
            await _emailService.SendVerificationEmailAsync(request.Email, code);
            return Ok(new { Message = "驗證碼已寄送" });
        }

        // 驗證輸入的驗證碼
        [HttpPost]
        public async Task<IActionResult> VerifyCode([FromBody] VerificationRequest request)
        {
            var isValid = _verificationService.ValidateCode(request.Email, request.Code);
            if (isValid)
            {
                var UserID = HttpContext.Session.GetString("UserID");
                var User = _context.TUsers.FirstOrDefault(c => c.FuserId.ToString() == UserID);

                User.Femail = request.Email;
                User.FgoogleId = null;
                User.FisEmailVerified = true;
                User.FupdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(new { Message = "驗證成功，信箱已成功更新" });
            }
            else 
            {
                return BadRequest(new { Message = "驗證碼錯誤或已過期" });
            }
                
        }
        

        //密碼加密
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }        

        public IActionResult Logout()
        {
            // 清除所有 Session 變數
            HttpContext.Session.Remove("UserID");  
            HttpContext.Session.Clear();  

            return RedirectToAction("Index", "FrontIndex"); 
        }


        //取得會員ID
        public IActionResult GetUser() 
        {
            var UserID = HttpContext.Session.GetString("UserID");
            var User = _context.TUsers.FirstOrDefault(c=>c.FuserId.ToString() == UserID);
            return Json(new { User });
        }

        //更新會員
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] TUser TUser)
        {
            var UserID = HttpContext.Session.GetString("UserID");
            var User = _context.TUsers.FirstOrDefault(c => c.FuserId.ToString() == UserID);
            HttpContext.Session.SetString("UserName", TUser.FfullName);
            if (User == null)
                return BadRequest("未登入請先登入");

            User.FfullName = TUser.FfullName;
            User.Fgender = TUser.Fgender;
            User.FidNumber = TUser.FidNumber;
            User.FphoneNumber = TUser.FphoneNumber;
            User.Fbirthday = TUser.Fbirthday;
            User.Faddress = TUser.Faddress;
            User.FcompanyNumber = TUser.FcompanyNumber;
            User.FupdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "更新成功" });
        }



        

        //取得作品集
        public async Task<IActionResult> GetPortfolio()
        {
            var UserID = HttpContext.Session.GetString("UserID");
            if(UserID == null)
                return BadRequest("未登入請先登入");

            var Portfolio = await _context.TImages.Where(c => c.FuserId.ToString() == UserID && c.Fcategory == "background").ToListAsync();
            return Json(new { Portfolio });
        }

        //新增作品集
        [HttpPost]
        public async Task<IActionResult> AddPortfolio(IFormFile file)
        {
            var UserID = HttpContext.Session.GetString("UserID");            

            if (file == null || file.Length == 0)
                return BadRequest(new {Message = "請選擇檔案"});

            // 允許的圖片格式
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { Message = "不支援的檔案格式，請上傳 PNG, JPG, JPEG 或 GIF" });
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "users", UserID, "worker", "background");
            var uploadsPath = Path.Combine("uploads", "users", UserID, "worker", "background");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var fileCompletePath = Path.Combine(uploadsFolder, uniqueFileName);
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            var TImage = new TImage
            {
                FuserId = Convert.ToInt32(UserID),
                Frole = "worker",
                Fcategory = "background",
                FimageName = uniqueFileName,
                FimagePath = filePath,
                FisMain = true,
                FcreatedAt = DateTime.Now
            };

            Console.WriteLine($"圖片路徑{fileCompletePath}");
            Console.WriteLine($"圖片路徑{filePath}");
            _context.TImages.Add(TImage);
            await _context.SaveChangesAsync();

            using (var stream = new FileStream(fileCompletePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = uniqueFileName;
            return Ok(new { Portfolio = TImage });
        }

        //刪除作品
        [HttpDelete]
        public async Task<IActionResult> DeletePortfolio(string? FimageId)
        {
            Console.WriteLine($"傳進來的ID：{FimageId}");
            try
            {
                var Image = await _context.TImages.FirstOrDefaultAsync(c => c.FimageId.ToString() == FimageId);
                if (Image == null)
                    return BadRequest(new { Message = "刪除失敗!"});
                _context.TImages.Remove(Image);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "刪除成功!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("捕捉到其他錯誤: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }
          
            

        //取得接案履歷資料
        public async Task<IActionResult> GetWorker()
        {
            var UserID = HttpContext.Session.GetString("UserID");
            if (UserID == null)
                return BadRequest("未登入請先登入");

            var Worker = await _context.TWorkers.FirstOrDefaultAsync(c => c.FuserId.ToString() == UserID);
            return Json(new { Worker });
        }

        //更新接案履歷資料
        [HttpPut]
        public async Task<IActionResult> AddWorker([FromBody] WorkerViewModel model)
        {
            var UserID = HttpContext.Session.GetString("UserID");
            if (UserID == null)
                return BadRequest("未登入請先登入");

            var Worker = await _context.TWorkers.FirstOrDefaultAsync(c => c.FuserId.ToString() == UserID);
            if (Worker == null)
            {
                var worker = new TWorker
                {
                    FuserId = Convert.ToInt32(UserID),
                    FcodeName = model.FcodeName,
                    FprofileDescription = model.FprofileDescription,
                    Fskills = model.Fskills
                };

                Console.WriteLine($"ID:{worker.FuserId},自介：{model.FprofileDescription}，技能：{model.Fskills}");
                _context.TWorkers.Add(worker);
                await _context.SaveChangesAsync();
                return Ok(new { Worker });
            }
            else 
            {
                Worker.FcodeName = model.FcodeName;
                Worker.FprofileDescription = model.FprofileDescription;               
                Worker.Fskills = model.Fskills;
                Worker.FisVerified = true;
                Console.WriteLine($"ID:{Worker.FuserId},自介：{model.FprofileDescription}，技能：{model.Fskills}");
                _context.TWorkers.Update(Worker);
                await _context.SaveChangesAsync();
                return Ok(new { Worker });
            }
            
        }


        //取得自己接案資訊
        public async Task<IActionResult> GetCase(string? status)
        {
            Console.WriteLine($"拿到的狀態：{status}");
            var UserID = HttpContext.Session.GetString("UserID");
            if (UserID == null)
                return BadRequest("未登入請先登入");
            if (status == "all")
            {
                var Case = await (from t in _context.Ttransactions
                                  join task in _context.TTasks on t.TaskId equals task.FtaskId
                                  join poster in _context.TUsers on t.PostUserId equals poster.FuserId
                                  where t.WorkUserId.ToString() == UserID
                                  select new
                                  { 
                                    t.TransactionId,
                                    t.TaskId,
                                    task.Ftitle,
                                    t.PostUserId,
                                    poster.FfullName,
                                    t.WorkUserId,
                                    t.Amount,
                                    t.Status,
                                    t.StartTime                                    
                                  }).ToListAsync();
                return Json(new { Case });
            }
            else
            {
                var Case = await (from t in _context.Ttransactions
                                  join task in _context.TTasks on t.TaskId equals task.FtaskId
                                  join poster in _context.TUsers on t.PostUserId equals poster.FuserId
                                  where t.WorkUserId.ToString() == UserID && t.Status == status
                                  select new
                                  {
                                      t.TransactionId,
                                      t.TaskId,
                                      task.Ftitle,
                                      t.PostUserId,
                                      poster.FfullName,
                                      t.WorkUserId,
                                      t.Amount,
                                      t.Status,
                                      t.StartTime
                                  }).ToListAsync();
                return Json(new { Case });
            }

        }

        //取得自己發案資料
        public async Task<IActionResult> GetProposals(string status) 
        {            
            var UserID = HttpContext.Session.GetString("UserID");
            if (UserID == null)
                return BadRequest("未登入請先登入");
            if (status == "all")
            {
                var Proposals = await _context.TTasks.Where(c => c.FposterId.ToString() == UserID).ToListAsync();
                return Json(new { Proposals });
            }
            else 
            {
                var Proposals = await _context.TTasks.Where(c => c.FposterId.ToString() == UserID && c.Fstatus== status).ToListAsync();
                return Json(new { Proposals });
            }                
            
        }

        //更新發案資料       
        public async Task<IActionResult> EditProposals(string? FtaskId)
        {
            var UserId = HttpContext.Session.GetString("UserID");            
            var td = await _context.TTasks.FirstOrDefaultAsync(c=>c.FtaskId.ToString() == FtaskId);
            if (UserId == null || td.FposterId.ToString() != UserId)
                return BadRequest("搜尋不到任務");
            // 取得所有的 `tCategory` 類別
            var categories = await _context.TCategories.ToListAsync();

            // 將 `tCategory` 資料放入 ViewBag
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.JobName
            }).ToList();

            TTaskDTO taskDTO = new TTaskDTO
            {
                FtaskId = td.FtaskId,
                FposterId = td.FposterId,
                FcategoryId = td.FcategoryId,
                Ftitle = td.Ftitle,
                Fdescription = td.Fdescription,
                Fbudget = td.Fbudget,
                Flocation = td.Flocation,
                FlocationDetail = td.FlocationDetail,
                Fmember = td.Fmember,
                Fphone = td.Fphone,
                Femail = td.Femail,
                Fstatus = td.Fstatus,
                Fdeadline = td.Fdeadline,
                FcreatedAt = td.FcreatedAt,
                FupdatedAt = td.FupdatedAt,
                FtimagePath = td.Ftimage,
            };            

            if (taskDTO == null)
                return BadRequest("搜尋不到任務");
            
            return View(taskDTO);
        }

        [HttpPost]
        public async Task<IActionResult> EditProposals(TTaskDTO taskDTO)
        {
            var UserId = HttpContext.Session.GetString("UserID");            
            var task = await _context.TTasks.FirstOrDefaultAsync(c => c.FtaskId == taskDTO.FtaskId);
            if (UserId == null || task.FposterId.ToString() != UserId)
                return BadRequest("搜尋不到任務");

            // 處理圖片上傳
            List<string> imagePaths = new List<string>();
            if (taskDTO.FtimageFiles != null && taskDTO.FtimageFiles.Count > 0)
            {                
                foreach (var imageFile in taskDTO.FtimageFiles)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName).ToLowerInvariant(); ;
                    var uploadPath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imagePaths.Add("/uploads/" + fileName);
                }
            }

            task.FcategoryId = taskDTO.FcategoryId;
            task.Ftitle = taskDTO.Ftitle;
            task.Fdescription = taskDTO.Fdescription;
            task.Fbudget = taskDTO.Fbudget;
            task.Flocation = taskDTO.Flocation;
            task.FlocationDetail = taskDTO.FlocationDetail;
            task.Fmember = taskDTO.Fmember;
            task.Fphone = taskDTO.Fphone;
            task.Femail = taskDTO.Femail;
            task.Fdeadline = taskDTO.Fdeadline;
            task.FupdatedAt = taskDTO.FupdatedAt;
            task.Fstatus = "待審核";
            task.Ftimage = string.Join(";", imagePaths);

            _context.TTasks.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("PersonalPage", "FProfileManage","Poster");
        }

        //刪除提案
        [HttpDelete]
        public async Task<IActionResult> deleteProposal(string? FtaskId) 
        {
            Console.WriteLine($"任務ID{FtaskId}");
            var UserId = HttpContext.Session.GetString("UserID");
            var task = await _context.TTasks.FirstOrDefaultAsync(c => c.FtaskId.ToString() == FtaskId);
            var reply = await _context.TConfirmReplys.Where(c => c.FtaskId.ToString() == FtaskId).ToListAsync();
            var followers = await _context.TTaskFollows.Where(c => c.FtaskId.ToString() == FtaskId).ToListAsync();
            if (UserId != null && task != null && UserId == task.FposterId.ToString())
            {
                _context.TConfirmReplys.RemoveRange(reply);
                _context.TTaskFollows.RemoveRange(followers);
                _context.TTasks.Remove(task);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "成功刪除" });
            }
            else 
            {
                return BadRequest(new { Message = "刪除失敗" });
            }
            
        }


        //更改密碼
        [HttpPost]
        public async Task<IActionResult> Updatepassword(
           [FromBody] UpdatePasswordViewModel PasswordVM)
        {
            Console.WriteLine(PasswordVM.OldPassword);
            Console.WriteLine(PasswordVM.NewPassword);
            Console.WriteLine(PasswordVM.ConfirmPassword);
            try
            {
                var UserID = HttpContext.Session.GetString("UserID");
                var User = _context.TUsers.FirstOrDefault(c => c.FuserId.ToString() == UserID);
                if (User == null)
                    return BadRequest(new { success = false, message = "使用者不存在" });
                if (string.IsNullOrEmpty(PasswordVM.OldPassword) ||string.IsNullOrEmpty(PasswordVM.NewPassword) || string.IsNullOrEmpty(PasswordVM.ConfirmPassword))
                {
                    return BadRequest(new { success = false, message = "密碼不得為空" });
                }

                if (HashPassword(PasswordVM.OldPassword)!= User.FpasswordHash)
                    return BadRequest(new { success = false, message = "舊密碼錯誤" }); 

                if (HashPassword(PasswordVM.OldPassword) == HashPassword(PasswordVM.NewPassword))
                {
                    return BadRequest(new { success = false, message = "新密碼不能與舊密碼相同" });
                }

                // 使用 Regex 檢查新密碼格式
                string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z]).{6,}$"; // 至少1個小寫、1個大寫、6個字元以上
                if (!Regex.IsMatch(PasswordVM.NewPassword, passwordPattern))
                {
                    return BadRequest(new { success = false, message = "新密碼格式錯誤，需包含至少 1 個大寫字母、1 個小寫字母，且長度至少 6 位" });
                }

                if (HashPassword(PasswordVM.NewPassword) != HashPassword(PasswordVM.ConfirmPassword))
                    return BadRequest(new { success = false, message = "密碼不一致" });

                User.FpasswordHash = HashPassword(PasswordVM.NewPassword);
                User.FupdatedAt = DateTime.Now;
                _context.SaveChanges();

                return Ok(new { success = true, message = "密碼更新成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "伺服器錯誤：" + ex.Message });
            }
    
        }


        //更新頭貼
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string oldImageUrl) 
        {
            Console.WriteLine(oldImageUrl);
            var UserID = HttpContext.Session.GetString("UserID");
            var User = _context.TUsers.FirstOrDefault(c => c.FuserId.ToString() == UserID);
            var oldImage = _context.TUsers.Where(c => c.FuserId.ToString() == UserID).Select(c=>c.FprofileImageUrl).FirstOrDefault();

            if (file == null || file.Length == 0)
                return BadRequest("請選擇檔案");

            // 允許的圖片格式
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("不支援的檔案格式，請上傳 PNG, JPG, JPEG 或 GIF");
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "Profile");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 刪除舊圖片
            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                var oldFilePath = Path.Combine(_env.WebRootPath, oldImageUrl.TrimStart('/')); // 確保是相對路徑
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            User.FprofileImageUrl = uniqueFileName;

            await _context.SaveChangesAsync();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = uniqueFileName;
            return Ok(new { imageUrl = fileUrl });            
        }

        //取得回覆
        [HttpGet]
        public async Task<IActionResult> GetReply(string? identity) 
        {
            Console.WriteLine($"身分確認{identity}");
            //取的UserID
            var UserID = HttpContext.Session.GetString("UserID");
            //取得回覆確認ID
            if (identity == "poster")
            {
                var replies = await (
                from reply in _context.TConfirmReplys
                join task in _context.TTasks on reply.FtaskId equals task.FtaskId
                join worker in _context.TWorkers on reply.FworkerId equals worker.FuserId
                where reply.FposterId.ToString() == UserID
                select new
                {
                    reply.FconfirmReplyId,
                    reply.FconfirmationType,
                    reply.FconfirmationStatus,
                    TaskTitle = task.Ftitle,         // 取得任務標題
                    WorkerFullName = worker.FcodeName,   // 取得接案者（tWorker 綁定 tUser）的全名
                    reply.Fremarks,
                    reply.FtaskId,
                    reply.FposterId,
                    reply.FworkerId
                }).ToListAsync();
                return Json(new { replies });
            } 
            else
            {
                var replies = await (
                from reply in _context.TConfirmReplys
                join task in _context.TTasks on reply.FtaskId equals task.FtaskId
                join worker in _context.TWorkers on reply.FworkerId equals worker.FuserId
                where reply.FworkerId.ToString() == UserID
                select new
                {
                    reply.FconfirmReplyId,
                    reply.FconfirmationType,
                    reply.FconfirmationStatus,
                    TaskTitle = task.Ftitle,         // 取得任務標題
                    WorkerFullName = worker.FcodeName,   // 取得接案者（tWorker 綁定 tUser）的全名
                    reply.Fremarks,
                    reply.FtaskId,
                    reply.FposterId,
                    reply.FworkerId
                }).ToListAsync();
                return Json(new { replies });
            }                        
        }

        //回覆確認
        public class ConfirmModel
        {
            public string Status { get; set; }
            public int FconfirmReplyId { get; set; }            
        }
        
        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody] ConfirmModel model) 
        {
            var userId = HttpContext.Session.GetString("UserID");
            var confirm = await _context.TConfirmReplys.FirstOrDefaultAsync(c => c.FconfirmReplyId == model.FconfirmReplyId);
            var task = await _context.TTasks.FirstOrDefaultAsync(c => c.FtaskId == confirm.FtaskId);
            
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            
            if (model.Status == "已確認")
            {
                //Console.WriteLine($"已確認：{model.Status},{model.FconfirmReplyId}" );

                /* 更新回復確認表 */
                confirm.FconfirmationStatus = "已確認";
                _context.TConfirmReplys.Update(confirm);

                /* 更新任務狀態 */
                task.Fstatus = "進行中";
                task.FupdatedAt = DateTime.Now;
                _context.TTasks.Update(task);

                /* 將同一筆任務ID，其他回復確認改為已被其他人接取 */
                await _context.TConfirmReplys
                 .Where(c => c.FtaskId == confirm.FtaskId && c.FworkerId != confirm.FworkerId)
                 .ExecuteUpdateAsync(s => s.SetProperty(r => r.FconfirmationStatus, r => "已拒絕"));


                /* 新增交易資料表 */
                var transaction = new Ttransaction
                {
                    TaskId = confirm.FtaskId,              // 對應 TTransaction 的 Task_ID
                    PostUserId = confirm.FposterId,        // 發案者 ID
                    WorkUserId = confirm.FworkerId,        // 從申請記錄中取得的接案者 ID
                    Amount = task.Fbudget,        // 任務金額
                    Status = "進行中",            // 初始交易狀態
                    StartTime = DateTime.Now,  // 交易開始時間
                    FinishTime = task.Fdeadline,  // 預計完成時間
                    Rating = null,                // 尚無評分
                    Review = null                 // 尚無評論
                };

                _context.Ttransactions.Add(transaction);
                await _context.SaveChangesAsync();
            }
            else if (model.Status == "已拒絕")
            {
                Console.WriteLine($"已拒絕：{model.Status},{model.FconfirmReplyId}");
                /* 更新回復確認表 */
                confirm.FconfirmationStatus = "已拒絕";
                _context.TConfirmReplys.Update(confirm);
                await _context.SaveChangesAsync();
            }
            else if (model.Status == "已取消")
            {
                Console.WriteLine($"已取消：{model.Status},{model.FconfirmReplyId}");
                /* 更新回復確認表 */
                var Transcation = await _context.Ttransactions.FirstOrDefaultAsync(c=>c.TaskId == task.FtaskId);
                Transcation.Status = "已取消";
                confirm.FconfirmationStatus = "已取消";
                _context.TConfirmReplys.Update(confirm);
                task.Fstatus = "已取消";
                await _context.SaveChangesAsync();
            }
            else if (model.Status == "拒絕取消")
            {
                Console.WriteLine($"拒絕取消：{model.Status},{model.FconfirmReplyId}");
                /* 更新回復確認表 */
                confirm.FconfirmationStatus = "拒絕取消";
                _context.TConfirmReplys.Update(confirm);               
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        //Notify通知查詢
        public async Task<IActionResult> GetNotify() 
        {
            var userId = HttpContext.Session.GetString("UserID");
            // 只查詢一次
            var unreadCount = await _context.TUserNotifications
                .Where(n => n.FUserId.ToString() == userId && !n.FIsRead)
                .CountAsync();

            Console.WriteLine($"回傳{unreadCount}");
            return Ok( new { unreadCount } );
        }

        public async Task<IActionResult> GetNotConfirm()
        {
            var userId = HttpContext.Session.GetString("UserID");
            // 只查詢一次
            var NotConfirmCount = await _context.TConfirmReplys
                .Where(n => n.FposterId.ToString() == userId && n.FconfirmationStatus=="待確認")
                .CountAsync();

            Console.WriteLine($"回傳{NotConfirmCount}");
            return Ok(new { NotConfirmCount });
        }
    }
}
