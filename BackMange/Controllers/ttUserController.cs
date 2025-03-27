using Microsoft.AspNetCore.Mvc;
using BackMange.Models;
using Microsoft.EntityFrameworkCore;
using BackMange.DTO;
using BackMange.ViewModels;
using System.Diagnostics;
using static BackMange.DTO.CUserDTO;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace BackMange.Controllers
{
    [Authorize(Roles = "Admin")]
    // 使用者管理控制器
    public class ttUserController : Controller
    {
        // 定義不同停權原因對應的停權天數
        private readonly Dictionary<SuspensionReason, int> _suspensionDays = new()
        {
            { SuspensionReason.ViolateCommunityGuidelines, 7 },  // 違反社群規範：7天
            { SuspensionReason.InappropriateContent, 3 },        // 不當內容：3天
            { SuspensionReason.SecurityConcern, 14 },            // 安全疑慮：14天
            { SuspensionReason.RepeatedViolations, 30 },         // 多次違規：30天
            { SuspensionReason.Harassment, 15 },                 // 騷擾行為：15天
            { SuspensionReason.Other, 7 },                       // 其他原因：7天
        };

        // 網站根目錄環境變數，用於處理檔案上傳
        IWebHostEnvironment _enviro = null!;

        // 建構函式，注入 IWebHostEnvironment 服務
        public ttUserController(IWebHostEnvironment p)
        {
            _enviro = p;
        }

        // 密碼雜湊方法（與 AccountController 共用邏輯）
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // 使用者列表頁面
        // 參數：搜尋關鍵字視圖模型、頁碼（預設第1頁）
        public IActionResult List(CAdminKeywordViewModel vm, int pageNumber = 1)
        {
            using GameCaseContext db = new GameCaseContext();
            string keyword = vm?.txtKeyword?.Trim() ?? "";

            // 設定每頁顯示 10 筆資料
            int pageSize = 10;

            // 準備基礎查詢，包含使用者類型資料
            IQueryable<TUser> query = db.TUsers.Include(u => u.FuserTypes);

            // 根據關鍵字建立搜尋條件
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p =>
                    // 搜尋基本資料
                    p.FfullName != null && p.FfullName.Contains(keyword) ||
                    p.Femail != null && p.Femail.Contains(keyword) ||
                    p.FphoneNumber != null && p.FphoneNumber.Contains(keyword) ||
                    p.Faddress != null && p.Faddress.Contains(keyword) ||
                    p.FidNumber != null && p.FidNumber.Contains(keyword) ||
                    p.FcompanyNumber != null && p.FcompanyNumber.Contains(keyword) ||
                    // 搜尋使用者類型
                    p.FuserTypes.Any(ut => ut.FuserTypeName.Contains(keyword)) ||
                    p.FloginType != null && p.FloginType.Contains(keyword) ||
                    // 搜尋狀態
                    (keyword == "啟用" && p.Fstatus == 1) ||
                    (keyword == "停用" && p.Fstatus == 2) ||
                    (keyword == "刪除" && p.Fstatus == 3) ||
                    (keyword == "未啟用" && p.Fstatus == 0) ||
                    // 搜尋停權原因
                    p.FsuspensionReason != null && p.FsuspensionReason.Contains(keyword) ||
                    // 搜尋性別
                    (keyword == "男" && p.Fgender == "M") ||
                    (keyword == "女" && p.Fgender == "F") ||
                    (keyword == "其他" && p.Fgender == "O")
                );
            }

            // 計算分頁資訊
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // 取得當前頁面的資料
            var datas = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 將資料轉換為 DTO
            List<CUserDTO> list = datas.Select(t => new CUserDTO(t)).ToList();

            // 設定分頁相關資訊到 ViewBag
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.Keyword = keyword;

            return View(list);
        }

        // 新增使用者頁面
        public IActionResult Create()
        {
            GameCaseContext db = new GameCaseContext();
            // 傳遞使用者類型列表到視圖
            ViewBag.UserTypes = db.TUserTypes.ToList();
            return View(new CUserDTO());
        }

        // 處理新增使用者
        [HttpPost]
        public IActionResult Create(TUser p, IFormFile? photoPath)
        {
            using (var db = new GameCaseContext())
            {
                // 檢查必要欄位
                if (string.IsNullOrEmpty(p.Femail) || string.IsNullOrEmpty(p.FpasswordHash))
                {
                    ModelState.AddModelError("", "Email 和密碼為必填欄位");
                    return View(new CUserDTO(p));
                }

                // 檢查 Email 是否已被使用
                if (db.TUsers.Any(u => u.Femail == p.Femail))
                {
                    ModelState.AddModelError("Femail", "此 Email 已被註冊");
                    return View(new CUserDTO(p));
                }

                // 檢查電話是否已被使用
                if (!string.IsNullOrEmpty(p.FphoneNumber) &&
                    db.TUsers.Any(u => u.FphoneNumber == p.FphoneNumber))
                {
                    ModelState.AddModelError("FphoneNumber", "此電話號碼已被使用");
                    return View(new CUserDTO(p));
                }

                try
                {
                    // 設定新使用者資料
                    p.FpasswordHash = HashPassword(p.FpasswordHash); // 密碼雜湊
                    p.FcreatedAt = DateTime.Now;
                    p.FupdatedAt = DateTime.Now;
                    p.Fstatus = 0;  // 未啟用狀態
                    p.FisEmailVerified = false;
                    p.FloginType = "Local";

                    // 新增使用者到資料庫
                    db.TUsers.Add(p);
                    db.SaveChanges();
                    TempData["success"] = "使用者新增成功";
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    // 新增失敗時顯示錯誤訊息
                    TempData["error"] = $"新增失敗: {ex.Message}";
                    return View(new CUserDTO(p));
                }
            }
        }

        // 刪除使用者（軟刪除）
        public IActionResult Delete(int? id)
        {
            if (id != null)
            {
                using GameCaseContext db = new GameCaseContext();
                TUser? x = db.TUsers.FirstOrDefault(c => c.FuserId == id);
                if (x != null)
                {
                    x.Fstatus = 3;  // 設定為刪除狀態
                    x.FupdatedAt = DateTime.Now;
                    db.SaveChanges();
                    TempData["success"] = "使用者已成功刪除";
                }
            }
            return RedirectToAction("List");
        }

        // 編輯使用者頁面
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            using GameCaseContext db = new GameCaseContext();
            // 取得使用者資料，包含使用者類型
            TUser? x = db.TUsers
                .Include(u => u.FuserTypes)
                .FirstOrDefault(c => c.FuserId == id);

            if (x == null)
                return RedirectToAction("List");

            // 準備視圖資料
            ViewBag.UserTypes = db.TUserTypes.ToList();
            CUserDTO userDTO = new CUserDTO(x);

            // 準備停用原因選項
            ViewBag.SuspensionReasons = Enum.GetValues<SuspensionReason>()
                .Select(r => new
                {
                    Value = (int)r,
                    Text = r.GetType()
                        .GetMember(r.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.Name ?? r.ToString(),
                    Days = _suspensionDays[r]
                });

            return View(userDTO);
        }

        // 處理編輯使用者
        [HttpPost]
        public IActionResult Edit(CUserDTO userDTO, IFormFile? photoPath)
        {
            using GameCaseContext db = new GameCaseContext();
            // 取得要編輯的使用者資料
            TUser? x = db.TUsers
                .Include(u => u.FuserTypes)
                .FirstOrDefault(c => c.FuserId == userDTO.FuserId);

            if (x != null)
            {
                // 更新基本資料
                x.FfullName = userDTO.FfullName;
                x.Femail = userDTO.Femail;
                x.FidNumber = userDTO.FidNumber;
                x.FcompanyNumber = userDTO.FcompanyNumber;
                x.FphoneNumber = userDTO.FphoneNumber;
                x.Faddress = userDTO.Faddress;
                x.Fbirthday = userDTO.Fbirthday;
                x.Fgender = userDTO.Fgender;
                x.FisEmailVerified = userDTO.FisEmailVerified;
                x.Fstatus = userDTO.Fstatus;

                // 處理停用相關資訊
                if (x.Fstatus == 2)  // 如果是停用狀態
                {
                    x.FsuspensionReason = userDTO.FsuspensionReason;
                    x.FsuspensionEndTime = userDTO.FsuspensionEndTime;
                }
                else
                {
                    x.FsuspensionReason = null;
                    x.FsuspensionEndTime = null;
                }

                // 更新時間戳記
                x.FupdatedAt = DateTime.Now;

                try
                {
                    db.SaveChanges();
                    TempData["success"] = "使用者資料更新成功";
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"更新失敗: {ex.Message}";
                    // 記錄錯誤訊息
                    Debug.WriteLine($"保存更新時發生錯誤: {ex.Message}");
                }
            }
            return RedirectToAction("List");
        }

        // 檢查 Email 是否已存在
        [HttpGet]
        public IActionResult CheckEmail(string email)
        {
            using (var db = new GameCaseContext())
            {
                bool exists = db.TUsers.Any(u => u.Femail == email);
                return Json(exists);
            }
        }
    }
}