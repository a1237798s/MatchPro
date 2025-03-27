using BackMange.Models;
using BackMange.DTO;
using BackMange.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
 

namespace BackMange.Controllers
{
    
    //test    
    public class ttAdminsController : Controller
    {
        private readonly GameCaseContext _db;
        //權限控管身分驗證用
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ttAdminsController(GameCaseContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //確認使用者是否存在
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["error"] = "登入失敗";
                ModelState.AddModelError("", "無效的登入嘗試"); // 不顯示 Email 錯誤，以免洩漏帳戶資訊
                return View(model);
            }

            //嘗試登入（驗證密碼）
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["success"] = "登入成功";
                return RedirectToAction("Index", "Home"); // 登入成功，導向首頁
            }
            else
            {
                TempData["error"] = "登入失敗";
                return View();
            }

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "ttAdmins"); // 登出後導向首頁
        }
        [Authorize(Roles = "Admin")]
        public IActionResult List(CAdminKeywordViewModel vm)
        {
            string keyword = vm?.txtKeyword;
            IEnumerable<TAdmin> datas;

            if (string.IsNullOrEmpty(keyword))
            {
                datas = _db.TAdmins.Include(a => a.Fstatus).ToList();
            }
            else
            {
                datas = _db.TAdmins
                    .Include(a => a.Fstatus)
                    .Where(p =>
                        p.FadminNo.Contains(keyword) ||
                        p.FfullName.Contains(keyword) ||
                        p.Femail.Contains(keyword) ||
                        p.FmobilePhone.Contains(keyword))
                    .ToList();
            }

            var adminDTOs = datas.Select(admin => new CAdminDTO(admin)).ToList();
            return View(adminDTOs);
        }

        // 新增管理員（顯示表單）
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // 生成新的管理員編號並傳給視圖
            ViewBag.NewAdminNo = GenerateNextAdminNo();
            return View(new CAdminDTO { FadminNo = ViewBag.NewAdminNo });
        }

        // 新增管理員（提交表單）
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CAdminDTO adminDTO)
        {
            if (!ModelState.IsValid)
            {
                // 如果驗證失敗，重新生成編號
                ViewBag.NewAdminNo = GenerateNextAdminNo();
                return View(adminDTO);
            }

            try
            {
                // 檢查 Email 是否已被使用
                if (_db.TAdmins.Any(u => u.Femail == adminDTO.Femail))
                {
                    ModelState.AddModelError("Femail", "此 Email 已被使用");
                    return View(adminDTO);
                }

                // 檢查手機號碼是否已被使用
                if (!string.IsNullOrEmpty(adminDTO.FmobilePhone) &&
                    _db.TAdmins.Any(u => u.FmobilePhone == adminDTO.FmobilePhone))
                {
                    ModelState.AddModelError("FmobilePhone", "此手機號碼已被使用");
                    return View(adminDTO);
                }

                // 確認密碼是否相符
                if (adminDTO.FadmPassword != adminDTO.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "密碼與確認密碼不相符");
                    return View(adminDTO);
                }

                var admin = new TAdmin
                {
                    FadminNo = adminDTO.FadminNo,  // 使用表單傳來的編號
                    FfullName = adminDTO.FfullName,
                    Femail = adminDTO.Femail,
                    FadmPassword = adminDTO.FadmPassword,
                    FmobilePhone = adminDTO.FmobilePhone,
                    FadminLevel = adminDTO.FadminLevel,
                    FstatusId = adminDTO.FstatusId,
                    FcreatedAt = DateTime.Now,
                    FupdatedAt = DateTime.Now
                };

                _db.TAdmins.Add(admin);
                _db.SaveChanges();

                TempData["success"] = "管理員新增成功";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["error"] = "新增失敗：" + ex.Message;
                ViewBag.NewAdminNo = GenerateNextAdminNo();  // 發生錯誤時重新生成編號
                return View(adminDTO);
            }
        }

        // 編輯管理員（顯示表單）
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            var admin = _db.TAdmins
                .Include(a => a.Fstatus)
                .FirstOrDefault(c => c.FadminId == id);

            if (admin == null)
                return RedirectToAction("List");

            var adminDTO = new CAdminDTO(admin);
            // 不要設置密碼，因為是編輯表單
            adminDTO.FadmPassword = null;
            adminDTO.ConfirmPassword = null;

            return View(adminDTO);
        }

        // 編輯管理員（提交表單）
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(CAdminDTO adminDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(adminDTO);
            }

            var admin = _db.TAdmins.FirstOrDefault(c => c.FadminId == adminDTO.FadminId);
            if (admin != null)
            {
                // 檢查 Email 是否已被其他用戶使用
                if (_db.TAdmins.Any(a => a.Femail == adminDTO.Femail && a.FadminId != adminDTO.FadminId))
                {
                    ModelState.AddModelError("Femail", "此 Email 已被使用");
                    return View(adminDTO);
                }

                // 檢查手機號碼是否已被其他用戶使用
                if (!string.IsNullOrEmpty(adminDTO.FmobilePhone) &&
                    _db.TAdmins.Any(a => a.FmobilePhone == adminDTO.FmobilePhone && a.FadminId != adminDTO.FadminId))
                {
                    ModelState.AddModelError("FmobilePhone", "此手機號碼已被使用");
                    return View(adminDTO);
                }

                admin.FfullName = adminDTO.FfullName;
                admin.Femail = adminDTO.Femail;

                // 處理密碼更新
                if (!string.IsNullOrEmpty(adminDTO.FadmPassword))
                {
                    // 確認密碼是否相符
                    if (adminDTO.FadmPassword != adminDTO.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "密碼與確認密碼不相符");
                        return View(adminDTO);
                    }
                    admin.FadmPassword = adminDTO.FadmPassword;
                }

                admin.FmobilePhone = adminDTO.FmobilePhone;
                admin.FadminLevel = adminDTO.FadminLevel;
                admin.FstatusId = adminDTO.FstatusId;
                admin.FupdatedAt = DateTime.Now;

                try
                {
                    _db.SaveChanges();
                    TempData["success"] = "管理員資料更新成功";
                }
                catch (Exception ex)
                {
                    TempData["error"] = "更新失敗：" + ex.Message;
                    return View(adminDTO);
                }
            }
            return RedirectToAction("List");
        }




        // 刪除管理員（變更帳號狀態為已刪除）
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            var admin = _db.TAdmins.FirstOrDefault(c => c.FadminId == id);
            if (admin != null && admin.FstatusId != 3)  // 
            {
                admin.FstatusId = 3;  //
                admin.FupdatedAt = DateTime.Now; 
                _db.SaveChanges();
            }

            return RedirectToAction("List");
        }
        private string GenerateNextAdminNo()
        {
            try
            {
                // 獲取目前最大的管理員編號
                var lastAdminNo = _db.TAdmins
                    .Where(a => a.FadminNo.StartsWith("A"))  // 確保只取 A 開頭的編號
                    .Select(a => a.FadminNo)
                    .OrderByDescending(no => no)
                    .FirstOrDefault();

                // 如果沒有現有編號，從 A001 開始
                if (string.IsNullOrEmpty(lastAdminNo))
                {
                    return "A001";
                }

                // 取得數字部分並產生下一個編號
                string numberPart = lastAdminNo.Substring(1);
                if (int.TryParse(numberPart, out int currentNumber))
                {
                    int nextNumber = currentNumber + 1;
                    return $"A00{nextNumber}";
                }

                // 如果解析失敗，返回預設值
                return "A001";
            }
            catch (Exception ex)
            {
                // 記錄錯誤
                Console.WriteLine($"生成管理員編號時發生錯誤: {ex.Message}");
                return "A001";
            }
        }


    }

}

