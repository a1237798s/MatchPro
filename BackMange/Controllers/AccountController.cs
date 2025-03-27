using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using BackMange.Models;
using BackMange.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Diagnostics;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using BackMange.Controllers.API;
using System.Threading.Tasks;




    namespace BackMange.Controllers
    {
        // 帳號管理控制器
        public class AccountController : Controller
        {
            private readonly GameCaseContext _db; // 資料庫上下文
            private readonly IConfiguration _configuration; // 配置設定
            private readonly INotificationService _notificationService; //系統通知用


        // 建構函式：注入相依服務
        public AccountController(GameCaseContext db, IConfiguration configuration, INotificationService notificationService)
            {
                _db = db;
                _configuration = configuration;
                //通知用
                _notificationService = notificationService;
            }

            // 顯示登入頁面
            public IActionResult Login()
            {
                return View();
            }

            // 顯示註冊頁面
            public IActionResult Register()
            {
                return View();
            }

            // 顯示註冊頁面2
            public IActionResult Register2()
            {
                return View();
            }

            // 開始 Google 登入流程
            public IActionResult GoogleLogin()
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = "/signin-google"
                };
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }

            // 處理 Google 登入回應
            [HttpPost]
            public async Task<IActionResult> GoogleResponse([FromForm] string credential)
            {
                try
                {
                    // 解析 JWT Token
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(credential) as JwtSecurityToken;

                    if (jsonToken == null)
                    {
                        Debug.WriteLine("Token 解析失敗");
                        return Json(new { success = false, message = "無效的 Token" });
                    }

                    // 從 Token 取得使用者資訊
                    string email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                    string name = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                    string googleId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                    Debug.WriteLine($"Google 登入資訊: Email={email}, Name={name}, GoogleId={googleId}");

                    if (string.IsNullOrEmpty(email))
                    {
                        return Json(new { success = false, message = "無法取得 Email" });
                    }

                    // 檢查使用者是否存在
                    var user = await _db.TUsers.FirstOrDefaultAsync(u => u.Femail == email);

                    if (user == null)
                    {
                        // 創建新使用者
                        user = new TUser
                        {
                            FfullName = name ?? "Google User",
                            Femail = email,
                            FidNumber = "",
                            FgoogleId = googleId,
                            FloginType = "Google",
                            Fstatus = 1,
                            FisEmailVerified = true,
                            FcreatedAt = DateTime.Now,
                            FupdatedAt = DateTime.Now,
                            FpasswordHash = null,
                            FphoneNumber = null,
                            Faddress = null,
                            Fbirthday = null,
                            Fgender = null,
                            FprofileImageUrl = null,
                            FlastLoginTime = DateTime.Now,
                            FsuspensionReason = null,
                            FsuspensionEndTime = null,
                            FcompanyNumber = null
                        };

                        try
                        {
                            _db.TUsers.Add(user);
                            await _db.SaveChangesAsync();
                            Debug.WriteLine("新用戶創建成功");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"創建用戶失敗: {ex.Message}");
                            Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                            return Json(new { success = false, message = "創建用戶失敗：" + ex.Message });
                        }
                    }
                    else
                    {
                        // 更新現有使用者資訊
                        user.FlastLoginTime = DateTime.Now;
                        user.FupdatedAt = DateTime.Now;
                        if (string.IsNullOrEmpty(user.FgoogleId))
                        {
                            user.FgoogleId = googleId;
                        }
                        await _db.SaveChangesAsync();
                    }

                    // 設置 Session
                    HttpContext.Session.SetString("UserID", user.FuserId.ToString());
                    HttpContext.Session.SetString("UserName", user.FfullName);
                    HttpContext.Session.SetString("UserLoggedIn", "true");
                    HttpContext.Session.SetString("UserEmail", user.Femail);

                    // 設置驗證 Cookie
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.FuserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FfullName),
                    new Claim(ClaimTypes.Email, user.Femail),
                    new Claim("LoginType", "Google")
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));

                    return Json(new
                    {
                        success = true,
                        redirectUrl = "/FrontIndex/Index"
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Google 登入失敗: {ex.Message}");
                    Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                    return Json(new { success = false, message = "登入失敗：" + ex.Message });
                }
            }

            // 顯示確認 Google 綁定頁面
            public IActionResult ConfirmGoogleLink()
            {
                if (TempData["GoogleId"] == null || TempData["Email"] == null)
                {
                    return RedirectToAction("Login");
                }
                return View();
            }

            // 確認 Google 帳號連結
            [HttpPost]
            public async Task<IActionResult> ConfirmGoogleLink(bool confirm)
            {
                if (!confirm)
                {
                    return RedirectToAction("Login");
                }

                string googleId = TempData["GoogleId"]?.ToString();
                string email = TempData["Email"]?.ToString();

                var user = await _db.TUsers.FirstOrDefaultAsync(u => u.Femail == email);
                if (user != null)
                {
                    user.FgoogleId = googleId;
                    user.FloginType = "Google";
                    user.FupdatedAt = DateTime.Now;
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Login");
            }

            // 檢查是否已註冊
            [HttpPost]
            public async Task<IActionResult> CheckIfRegistered([FromBody] CheckRegistrationRequest request)
            {
                if (request == null)
                {
                    return BadRequest("Invalid request");
                }

                var isEmailRegistered = await _db.TUsers.AnyAsync(u => u.Femail == request.Femail);
                var isPhoneRegistered = await _db.TUsers.AnyAsync(u => u.FphoneNumber == request.FphoneNumber);
                var isIdRegistered = await _db.TUsers.AnyAsync(u => u.FidNumber == request.FidNumber);

                return Ok(new
                {
                    isEmailRegistered,
                    isPhoneRegistered,
                    isIdRegistered
                });
            }

            // 處理註冊
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DoRegister([FromBody] RegisterViewModel model)
            {
                var outModel = new RegisterResultViewModel();

                try
                {
                    // 驗證模型
                    if (!ModelState.IsValid)
                    {
                        outModel.ErrMsg = string.Join("; ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage));
                        return Json(outModel);
                    }

                    // 檢查重複註冊
                    var isEmailRegistered = await _db.TUsers.AnyAsync(u => u.Femail == model.Femail);
                    var isPhoneRegistered = await _db.TUsers.AnyAsync(u => u.FphoneNumber == model.FphoneNumber);
                    var isIdRegistered = await _db.TUsers.AnyAsync(u => u.FidNumber == model.FidNumber);

                    if (isEmailRegistered)
                    {
                        outModel.ErrMsg = "此 Email 已被註冊";
                        return Json(outModel);
                    }
                    if (isPhoneRegistered)
                    {
                        outModel.ErrMsg = "此電話已被註冊";
                        return Json(outModel);
                    }
                    if (isIdRegistered)
                    {
                        outModel.ErrMsg = "此身分證字號已被註冊";
                        return Json(outModel);
                    }

                    // 產生驗證 Token
                    string token = GenerateVerificationToken();
                    Console.WriteLine($"新註冊用戶 {model.Femail} 生成 Token: {token}");

                    // 加密密碼
                    string passwordHash = HashPassword(model.FpasswordHash);
                    Console.WriteLine($"加密後密碼: {passwordHash}");

                    // 建立新使用者
                    var newUser = new TUser
                    {
                        FfullName = model.FfullName,
                        Femail = model.Femail,
                        FpasswordHash = passwordHash,
                        FphoneNumber = model.FphoneNumber,
                        Faddress = model.Faddress,
                        FidNumber = model.FidNumber,
                        FloginType = "Local",
                        Fstatus = 0,
                        FisEmailVerified = false,
                        FcreatedAt = DateTime.Now,
                        FupdatedAt = DateTime.Now
                    };

                    _db.TUsers.Add(newUser);
                    await _db.SaveChangesAsync();

                    // 建立驗證記錄
                    var verification = new TVerification
                    {
                        FuserId = newUser.FuserId,
                        Ftoken = token,
                        FtokenType = "EmailVerification",
                        FtokenSentTime = DateTime.Now,
                        FexpirationTime = DateTime.Now.AddDays(1),
                        FisUsed = false
                    };

                    _db.TVerifications.Add(verification);
                    await _db.SaveChangesAsync();

                    // 發送驗證信
                    SendVerificationEmail(model.Femail, token);

                //此處為發送通知
                #region === 發送通知 ===
                // 準備通知：檢查是否已發送過系統通知
                var existingNotification = await _db.TUserNotifications
                    .FirstOrDefaultAsync(n =>
                        n.FUserId == newUser.FuserId &&
                        n.FRelatedId == null &&
                        n.FNotifyType == "System" &&
                        n.FMessage.Contains("歡迎加入"));

                if (existingNotification == null)
                {
                    // 發送通知給接案者
                    var notification = new NotiObj
                    {
                        UserId = newUser.FuserId,  // 接案者ID
                        Content = $"用戶: {newUser.FfullName} 歡迎加入 Match Pro ! 建議您先完善個人資料，以便使用接案 / 發案功能。 ",
                        Type = "System",              // 使用 Payment 類型
                        RelatedId = null,           // 相關案件ID
                        SenderId = null,     // 案主ID
                    };

                    var (success, message, _) = await _notificationService.SendNotificationAsync(notification);

                    if (!success) Console.WriteLine($"狀態更新成功，但通知發送失敗：{message}");
                    else Console.WriteLine("已通知發案者案件已通過!");
                }
                #endregion === 發送通知 END===


                outModel.ResultMsg = "註冊成功，請查收驗證信件";
                    return Json(outModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"註冊失敗: {ex.Message}");
                    outModel.ErrMsg = "註冊失敗：" + ex.Message;
                    return Json(outModel);
                }
            }

            // 產生驗證 Token
            private string GenerateVerificationToken()
            {
                // 1. 創建安全的隨機數生成器
                using (var crypto = RandomNumberGenerator.Create())
                {
                    // 2. 創建一個 16 位元組的陣列
                    byte[] tokenBytes = new byte[16];

                    // 3. 使用隨機數生成器填充陣列
                    crypto.GetBytes(tokenBytes);

                    // 4. 將位元組陣列轉換為 Base64 字串，並移除特殊字元
                    return Convert.ToBase64String(tokenBytes)
                        .Replace("+", "")    // 移除 + 符號
                        .Replace("/", "")    // 移除 / 符號
                        .Replace("=", "");   // 移除 = 符號
                }
            }

            // 密碼雜湊
            private string HashPassword(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Convert.ToBase64String(bytes);
                }
            }

            // 發送驗證信
            private void SendVerificationEmail(string email, string token)
            {
                string verifyUrl = Url.Action("VerifyEmail", "Account",
                    new { token = token }, HttpContext.Request.Scheme);
                string subject = "請驗證您的 Email";
                string body = $"請點擊以下連結驗證您的帳戶：<a href='{verifyUrl}'>驗證 Email</a>";

                try
                {
                    MailMessage MM = new MailMessage();
                    MM.From = new MailAddress("coderforever145@gmail.com");
                    MM.Priority = MailPriority.Normal;
                    MM.Subject = subject;
                    MM.IsBodyHtml = true;
                    MM.To.Add(new MailAddress(email));
                    MM.Body = body;

                    using (SmtpClient Mysmtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        Mysmtp.Credentials = new System.Net.NetworkCredential(
                            "coderforever145@gmail.com", "ztwu xwhq flym xihu");
                        Mysmtp.EnableSsl = true;
                        Mysmtp.Send(MM);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Email 發送失敗: {ex.Message}");
                }
            }

            // 重新發送驗證信
            [HttpPost]
            public async Task<IActionResult> SendVerificationEmail([FromBody] AccountViewModels model)
            {
                try
                {
                    var user = await _db.TUsers.FirstOrDefaultAsync(u => u.Femail == model.Femail);
                    if (user == null)
                    {
                        return NotFound("找不到該 Email");
                    }

                    // 刪除舊的驗證 Token
                    var existingTokens = await _db.TVerifications
                        .Where(v => v.FuserId == user.FuserId &&
                                   v.FtokenType == "EmailVerification")
                        .ToListAsync();

                    if (existingTokens.Any())
                    {
                        _db.TVerifications.RemoveRange(existingTokens);
                        await _db.SaveChangesAsync();
                    }

                    // 產生新的 Token
                    string token = GenerateVerificationToken();

                    // 建立新的驗證記錄
                    var verification = new TVerification
                    {
                        FuserId = user.FuserId,
                        Ftoken = token,
                        FtokenType = "EmailVerification",
                        FtokenSentTime = DateTime.Now,
                        FexpirationTime = DateTime.Now.AddDays(1),
                        FisUsed = false
                    };

                    _db.TVerifications.Add(verification);
                    await _db.SaveChangesAsync();

                    // 發送驗證信
                    SendVerificationEmail(user.Femail, token);

                    return Ok("驗證信已發送");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"發送驗證信失敗: {ex.Message}");
                    return BadRequest("發送驗證信失敗，請稍後再試");
                }
            }

            // 驗證 Email
            [HttpGet]
            public async Task<IActionResult> VerifyEmail(string token)
            {
                var verification = await _db.TVerifications
                    .FirstOrDefaultAsync(v => v.Ftoken == token &&
                                            v.FtokenType == "EmailVerification" &&
                                            v.FisUsed == false);

                if (verification == null || verification.FexpirationTime < DateTime.Now)
                {
                    return BadRequest("驗證失敗，無效的 Token 或 Token 已過期");
                }

                var user = await _db.TUsers.FindAsync(verification.FuserId);
                if (user == null)
                {
                    return BadRequest("找不到對應的用戶");
                }

                user.FisEmailVerified = true;
                user.Fstatus = 1;
                await _db.SaveChangesAsync();

                verification.FisUsed = true;
                verification.FusedTime = DateTime.Now;
                await _db.SaveChangesAsync();

                return Content("Email 驗證成功！您的帳戶已啟用");
            }

            // 通用發送 Email 方法
            private void SendEmail(string email, string subject, string body)
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("coderforever145@gmail.com");
                mailMessage.Priority = MailPriority.Normal;
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(email));
                mailMessage.Body = body;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(
                        "coderforever145@gmail.com", "ztwu xwhq flym xihu");
                    smtpClient.EnableSsl = true;
                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Email 發送失敗: {ex.Message}");
                    }
                }
            }

            // 忘記密碼處理
            [HttpPost]
            public async Task<IActionResult> ForgotPassword([FromBody] AccountViewModels model)
            {
                try
                {
                    var user = await _db.TUsers.FirstOrDefaultAsync(u => u.Femail == model.Femail);
                    if (user == null)
                    {
                        return NotFound("找不到該 Email");
                    }

                    // 刪除舊的重設密碼 Token
                    var existingTokens = await _db.TVerifications
                        .Where(v => v.FuserId == user.FuserId &&
                                   v.FtokenType == "PasswordReset")
                        .ToListAsync();

                    if (existingTokens.Any())
                    {
                        _db.TVerifications.RemoveRange(existingTokens);
                        await _db.SaveChangesAsync();
                    }

                    // 產生新的 Token
                    string token = GenerateVerificationToken();

                    // 建立新的驗證記錄
                    var verification = new TVerification
                    {
                        FuserId = user.FuserId,
                        Ftoken = token,
                        FtokenType = "PasswordReset",
                        FtokenSentTime = DateTime.Now,
                        FexpirationTime = DateTime.Now.AddHours(1),
                        FisUsed = false
                    };

                    _db.TVerifications.Add(verification);
                    await _db.SaveChangesAsync();

                    // 發送重設密碼信
                    string resetUrl = Url.Action("Login", "Account",
                        new { token = token }, Request.Scheme);
                    SendEmail(user.Femail, "重設密碼",
                        $"請點擊 <a href='{resetUrl}'>這裡</a> 設定新密碼");

                    return Ok("重設密碼郵件已發送");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"系統錯誤: {ex.Message}");
                }
            }

            // 重設密碼
            [HttpPost]
            public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
            {
                var verification = await _db.TVerifications
                    .FirstOrDefaultAsync(v => v.Ftoken == model.Token &&
                                            v.FtokenType == "PasswordReset" &&
                                            (!v.FisUsed.HasValue || !v.FisUsed.Value));

                if (verification == null || verification.FexpirationTime < DateTime.Now)
                {
                    return BadRequest("無效或過期的 Token");
                }

                var user = await _db.TUsers.FindAsync(verification.FuserId);
                if (user == null)
                {
                    return BadRequest("找不到對應的用戶");
                }

                user.FpasswordHash = HashPassword(model.Password);
                await _db.SaveChangesAsync();

                verification.FisUsed = true;
                verification.FusedTime = DateTime.Now;
                await _db.SaveChangesAsync();

                return Ok("密碼已成功更新");
            }

            // 登入處理
            [HttpPost]
            public async Task<IActionResult> DoLogin([FromBody] AccountViewModels model)
            {
                var outModel = new LoginResultViewModel();

                try
                {
                    if (string.IsNullOrEmpty(model.Femail) ||
                        string.IsNullOrEmpty(model.FpasswordHash))
                    {
                        outModel.ErrMsg = "請輸入資料";
                        return Json(outModel);
                    }

                    var user = await _db.TUsers
                        .FirstOrDefaultAsync(u => u.Femail == model.Femail);

                    Console.WriteLine(user);

                    if (user == null || user.FpasswordHash != HashPassword(model.FpasswordHash))
                    {
                        Debug.WriteLine("User not found in database");
                        outModel.ErrMsg = "帳號或密碼錯誤";
                        return Json(outModel);
                    }

                    // 處理未啟用狀態
                    if (user.Fstatus == 0)
                    {
                        await LoginSuccess(user, outModel);
                        outModel.NeedVerify = true;
                        outModel.ResultMsg = "登入成功，請盡快完成信箱驗證以啟用帳號";
                        return Json(outModel);
                    }

                    // 處理各種帳號狀態
                    switch (user.Fstatus)
                    {
                        case 1: // 正常
                            await LoginSuccess(user, outModel);
                            if (user.FisEmailVerified.HasValue && !user.FisEmailVerified.Value)
                            {
                                outModel.NeedVerify = true;
                                outModel.ResultMsg = "登入成功，請盡快完成信箱驗證";
                            }
                            else
                            {
                                outModel.ResultMsg = "登入成功";
                            }
                            break;

                        case 2: // 停用
                            if (user.FsuspensionEndTime.HasValue)
                            {
                                var remainingDays = (user.FsuspensionEndTime.Value - DateTime.Now).Days;
                                if (remainingDays > 0)
                                {
                                    outModel.ErrMsg = $"帳號已被停用，還有 {remainingDays} 天可以解除停用";
                                }
                                else
                                {
                                    user.Fstatus = 1;
                                    user.FsuspensionReason = null;
                                    user.FsuspensionEndTime = null;
                                    await _db.SaveChangesAsync();
                                    await LoginSuccess(user, outModel);
                                    outModel.ResultMsg = "停用期已結束，帳號已恢復正常";
                                }
                            }
                            else
                            {
                                outModel.ErrMsg = $"帳號已被停用，原因：{user.FsuspensionReason ?? "未說明"}";
                            }
                            break;

                        case 3: // 刪除
                            outModel.ErrMsg = "帳號不存在";
                            break;

                        default:
                            await LoginSuccess(user, outModel);
                            outModel.NeedVerify = true;
                            outModel.ResultMsg = "登入成功，但帳號狀態異常，請聯繫管理員";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                    outModel.ErrMsg = "系統錯誤：" + ex.Message;
                }

                return Json(outModel);
            }

            // 登入成功處理
            private async Task LoginSuccess(TUser user, LoginResultViewModel outModel)
            {
                try
                {
                    HttpContext.Session.SetString("UserID", user.FuserId.ToString());
                    HttpContext.Session.SetString("UserName", user.FfullName);
                    HttpContext.Session.SetString("UserLoggedIn", "true");

                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.FuserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FfullName),
                    new Claim(ClaimTypes.Email, user.Femail),
                    new Claim("LoginType", user.FloginType ?? "Local")
                };

                    var identity = new ClaimsIdentity(claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));

                    outModel.ResultMsg = "登入成功";
                    Debug.WriteLine("登入成功，Session 設置完成");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"LoginSuccess 發生錯誤: {ex.Message}");
                    outModel.ErrMsg = "登入過程發生錯誤：" + ex.Message;
                }
            }

            // 取得當前使用者資訊
            [HttpGet]
            public IActionResult GetCurrentUser()
            {
                var userName = HttpContext.Session.GetString("UserName");
                var userId = HttpContext.Session.GetString("UserID");

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "使用者尚未登入" });
                }

                return Ok(new
                {
                    UserName = userName,
                    UserID = userId
                });
            }

            // 登出處理
            public async Task<IActionResult> Logout()
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
        }
    }

