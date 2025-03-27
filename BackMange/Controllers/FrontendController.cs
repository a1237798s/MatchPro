using BackMange.DTO;
using BackMange.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackMange.Controllers
{
    public class FrontendController : Controller
    {
        private readonly GameCaseContext _context;
        private readonly IWebHostEnvironment _env;
        public FrontendController(GameCaseContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }




        [HttpGet]
        public async Task<IActionResult> missionList2(string searchQuery, string sortOrder, int? categoryId, string selectedCity, int pageNumber = 1)

        {









            int pageSize = 16; // 每頁顯示 16 筆
            var query = _context.TTasks
                .Where(c => c.Fstatus == "發佈中") // 只顯示審核通過案件
                .AsQueryable(); // 轉換為可查詢物件

            // 取得所有類別並存入 ViewBag
            ViewBag.Categories = new SelectList(await _context.TCategories
         .Select(c => new { c.Id, c.JobName })
         .ToListAsync(), "Id", "JobName");

            // 如果有輸入搜尋關鍵字，則篩選標題或描述符合的案件
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(t => t.Ftitle.Contains(searchQuery) || t.Fdescription.Contains(searchQuery));
            }

            // **加入類別篩選**
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(t => t.FcategoryId == categoryId.Value);
            }

            // ✅ 設定城市篩選
            if (!string.IsNullOrEmpty(selectedCity))
            {
                string normalizedCity = selectedCity.Trim().ToLower();
                query = query.Where(t => t.Flocation.Trim().ToLower() == normalizedCity);

            }

            // **排序邏輯**
            query = query
                .OrderByDescending(t => t.FcreatedAt); // **依創建時間降序排列**

            // 根據排序條件進行排序
            query = sortOrder switch
            {
                "price_desc" => query.OrderByDescending(c => c.Fbudget), // 價格由高到低
                "price_asc" => query.OrderBy(c => c.Fbudget),            // 價格由低到高
                _ => query.OrderByDescending(c => c.FcreatedAt)          // 預設依建立時間降序
            };

            // ✅ 取得所有不同的 `Flocation` 值作為城市列表
            var cities = await _context.TTasks
                .Where(t => !string.IsNullOrEmpty(t.Flocation)) // 避免空值
                .Select(t => t.Flocation)
                .Distinct()
                .ToListAsync();

            // ✅ 確保 `ViewBag.Cities` 有資料
            ViewBag.Cities = cities.Select(city => new SelectListItem
            {
                Value = city,
                Text = city,
                Selected = city == selectedCity // 自動選中目前選擇的城市
            }).ToList();

            // 計算總案件數量
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // 分頁查詢
            var tasks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TTaskDTO
                {
                    FtaskId = t.FtaskId,
                    Ftitle = t.Ftitle,
                    FposterId = t.FposterId,
                    Fdescription = t.Fdescription,
                    Fbudget = t.Fbudget,
                    Fstatus = t.Fstatus,
                    Fdeadline = t.Fdeadline,
                    FcreatedAt = t.FcreatedAt,
                    FupdatedAt = t.FupdatedAt,
                    FtimagePath = t.Ftimage,
                    Flocation = t.Flocation,
                    JobName = _context.TCategories
            .Where(c => c.Id == t.FcategoryId)
            .Select(c => c.JobName)
            .FirstOrDefault(), // 取得對應類別名稱
                    FollowCount = _context.TTaskFollows.Count(f => f.FtaskId == t.FtaskId),

                    // 計算任務剩餘天數
                    RemainingDays = (t.Fdeadline - DateTime.Now).Days,

                    // 加入發案人資訊 (從 `TPosters` 和 `TUsers` 取得)
                    FCompanyName = _context.TPosters
                            .Where(p => p.FuserId == t.FposterId)
                            .Select(p => p.FcompanyName)
                            .FirstOrDefault(),

                    FFullName = _context.TUsers
                            .Where(u => u.FuserId == t.FposterId)
                            .Select(u => u.FfullName)
                            .FirstOrDefault(),

                    FProfileImageUrl = _context.TUsers
                            .Where(u => u.FuserId == t.FposterId)
                            .Select(u => u.FprofileImageUrl)
                            .FirstOrDefault()


                })
                .ToListAsync();

            // 將分頁變數存入 ViewBag
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SelectedCategoryId = categoryId;

            return View(tasks);
        }





        [HttpGet]
        public async Task<IActionResult> missionCreate()
        {
            var userId = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // 查找會員資訊
            var user = await _context.TUsers.FirstOrDefaultAsync(u => u.FuserId == int.Parse(userId));

            // 查找發案者資訊（tPosters 關聯 tUsers）
            var poster = await _context.TPosters.FirstOrDefaultAsync(p => p.FuserId == int.Parse(userId));

            if (user == null || poster == null)
            {
                return NotFound("找不到對應的會員或發案者資料");
            }

            // 取得所有的 `tCategory` 類別
            var categories = await _context.TCategories.ToListAsync();

            // 將 `tCategory` 資料放入 ViewBag
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.JobName
            }).ToList();

            // 建立 DTO，並填充 `User` 和 `Poster` 資訊
            var dto = new TTaskDTO
            {
                User = user,
                Poster = poster,
                FposterId = poster.FuserId // 確保 `FposterId` 正確填入
            };

            return View(dto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> missionCreate(TTaskDTO dto)
        {
            // 取得登入的 UserID
            var userId = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(userId))
            {
                // 未登入則導向登入頁
                return RedirectToAction("Login", "Account");
            }

            // 確保 `FBudget` 是有效數值
            if (dto.Fbudget <= 0)
            {
                ModelState.AddModelError("Fbudget", "預算金額必須大於 0");
                return View(dto);
            }
            // 透過 UserID 查找會員資訊
            var user = await _context.TUsers.FirstOrDefaultAsync(u => u.FuserId == int.Parse(userId));

            // 查找發案者資訊（tPosters 關聯 tUsers）
            var poster = await _context.TPosters.FirstOrDefaultAsync(p => p.FuserId == int.Parse(userId));

            if (user == null || poster == null)
            {
                return NotFound("找不到對應的會員或發案者資料");
            }

            // 確保 FposterId 來自發案者資訊
            dto.FposterId = poster.FuserId;

            // 處理圖片上傳
            List<string> imagePaths = new List<string>();
            if (dto.FtimageFiles != null && dto.FtimageFiles.Count > 0)
            {
                foreach (var imageFile in dto.FtimageFiles)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imagePaths.Add("/uploads/" + fileName);
                }
            }

            // 建立任務物件並存入資料庫
            var task = new TTask
            {
                FtaskId = dto.FtaskId,
                FposterId = dto.FposterId, // 來自 `tPosters` 而非直接用 `UserID`
                FcategoryId = dto.FcategoryId,
                Ftitle = dto.Ftitle,
                Fdescription = dto.Fdescription,
                Fbudget = dto.Fbudget,
                Flocation = dto.Flocation,
                FlocationDetail = dto.FlocationDetail,
                Fmember = dto.Fmember,
                Fphone = dto.Fphone,
                Femail = dto.Femail,
                Fstatus = "待審核",
                Fdeadline = dto.Fdeadline,
                FcreatedAt = DateTime.Now,
                FupdatedAt = DateTime.Now,
                Ftimage = string.Join(";", imagePaths) // 多張圖片用分號存入

            };

            _context.TTasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("missionList2");
        }


        public async Task<IActionResult> missionDetail(int Id)
        {
            var userId = HttpContext.Session.GetString("UserID");


            var task = _context.TTasks
            .Where(s => s.FtaskId == Id)
            .Select(td => new TTaskDTO
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

                FAcceptedUserId = _context.Ttransactions
                .Where(t => t.TaskId == td.FtaskId)
                .Select(t => t.WorkUserId)
                .FirstOrDefault(),
                CurrentUserId = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId), // 當前登入用戶 ID
                Fdeadline = td.Fdeadline,
                FcreatedAt = td.FcreatedAt,
                FupdatedAt = td.FupdatedAt,
                FtimagePath = td.Ftimage,// 將資料庫中的圖片路徑賦值給 FtimagePath
                                         // **取得分類名稱**
                JobName = _context.TCategories
                .Where(c => c.Id == td.FcategoryId)
                .Select(c => c.JobName)
                .FirstOrDefault(),
                // **取得發案者的會員資訊**
                FFullName = _context.TUsers
                .Where(u => u.FuserId == td.FposterId)
                .Select(u => u.FfullName)
                .FirstOrDefault(),

                // **取得發案者頭像（拼接 URL，而不是檔案路徑）**
                FProfileImageUrl = _context.TUsers
                .Where(u => u.FuserId == td.FposterId)
                .Select(u => u.FprofileImageUrl != null
                    ? u.FprofileImageUrl
                    : "/images/default-avatar.png")
                .FirstOrDefault(),


                // **取得發案者的公司資訊**
                FCompanyName = _context.TPosters
                .Where(p => p.FuserId == td.FposterId)
                .Select(p => p.FcompanyName)
                .FirstOrDefault(),

                TransactionId = _context.Ttransactions
                      .Where(t => t.TaskId == td.FtaskId)
                      .Select(t => t.TransactionId)
                      .FirstOrDefault(),// 取得對應的交易 ID

                // ⭐ 取得評價資訊
                Rating = _context.Ttransactions
              .Where(t => t.TaskId == td.FtaskId)
              .Select(t => (int?)t.Rating) // `int?` 避免 `null` 轉型錯誤
              .FirstOrDefault(),

                Review = _context.Ttransactions
              .Where(t => t.TaskId == td.FtaskId)
              .Select(t => t.Review)
              .FirstOrDefault()

            })
            .FirstOrDefault();

            Console.WriteLine($"貼文者頭貼{task.FProfileImageUrl}");

            if (task == null)
            {
                return NotFound();
            }
            var ProfileImagePath = Path.Combine("\\", "uploads", "Profile", task.FProfileImageUrl);
            Console.WriteLine($"貼文者網址{ProfileImagePath}");
            ViewBag.img = ProfileImagePath;
            ViewBag.PosterId = task.FposterId;  // 將 PosterId 傳到 View
            ViewBag.UserId = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
            //Console.WriteLine($"PosterId: {ViewBag.PosterId}");

            return View(task);


        }



        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyForTask(int? taskId)
        {
            // 取得登入的 UserID
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                // 未登入則導向登入頁
                //return RedirectToAction("Login", "Account");
                return Json(new { success = false, message = "請先登入再進行操作！", redirect = Url.Action("Login", "Account") });
            }

            int userIdInt = int.Parse(userId);
            // 查詢任務
            var task = await _context.TTasks.FindAsync(taskId);
            if (task == null || task.Fstatus != "發佈中")
            {
                //return NotFound(); // 任務不存在或狀態不符時回傳 404
                return Json(new { success = false, message = "任務不存在或無法申請！" });

            }

            bool URU = task.FposterId == userIdInt;
            if (URU)
            {
                return Json(new { success = false, message = "您不能接自己的任務唷！" });
            }

            // 檢查是否已有待確認的申請記錄
            bool alreadyApplied = _context.TConfirmReplys.Any(cr =>
                cr.FtaskId == taskId &&
                cr.FworkerId == userIdInt &&

                cr.FconfirmationStatus == "待確認");
            if (alreadyApplied)
            {
                //return BadRequest("您已送出申請，請等待發案者回覆！");
                return Json(new { success = false, message = "您已送出申請，請等待發案者回覆！" });
            }

            // 建立確認回覆記錄，等待發案者同意
            var confirmReply = new TConfirmReply
            {
                FtaskId = (int)taskId,
                FposterId = task.FposterId,   // 發案者ID
                FworkerId = userIdInt,          // 申請者ID
                FconfirmationType = "接案確認",
                FconfirmationStatus = "待確認",
                Fremarks = "等待發案者確認申請"
            };

            _context.TConfirmReplys.Add(confirmReply);
            await _context.SaveChangesAsync();

            // 可額外進行通知發案者（例如寄送通知信或系統訊息）的處理

            //return RedirectToAction("missionDetail", new { Id = taskId });
            return Json(new { success = true, message = "申請成功，等待發案者回覆！" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmTaskApplication(int taskId)
        {
            // 取得目前登入的發案者 UserID
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            int posterId = int.Parse(userId);

            // 查詢任務，並確認該任務屬於目前發案者
            var task = await _context.TTasks.FindAsync(taskId);
            if (task == null || task.FposterId != posterId)
            {
                return NotFound("任務不存在或您無權操作");
            }

            // 從 tConfirmReplys 取得對應該任務且狀態為「待確認」的申請記錄
            var confirmReply = await _context.TConfirmReplys.FirstOrDefaultAsync(cr =>
                cr.FtaskId == taskId &&
                cr.FconfirmationStatus == "待確認");
            if (confirmReply == null)
            {
                return NotFound("找不到待確認的申請記錄");
            }

            // 從申請記錄中取得接案者 ID
            int workerId = confirmReply.FworkerId;

            // 更新申請記錄狀態，表示發案者已確認該申請
            confirmReply.FconfirmationStatus = "已確認";
            _context.TConfirmReplys.Update(confirmReply);

            // 建立交易記錄
            var transaction = new Ttransaction
            {
                TaskId = taskId,              // 對應 TTransaction 的 Task_ID
                PostUserId = posterId,        // 發案者 ID
                WorkUserId = workerId,        // 從申請記錄中取得的接案者 ID
                Amount = task.Fbudget,        // 任務金額
                Status = "進行中",            // 初始交易狀態
                StartTime = DateTime.UtcNow,  // 交易開始時間
                FinishTime = task.Fdeadline,  // 預計完成時間
                Rating = null,                // 尚無評分
                Review = null                 // 尚無評論
            };
            _context.Ttransactions.Add(transaction);

            // 更新任務狀態，反映目前進入雙方確認階段
            task.Fstatus = "進行中";
            _context.TTasks.Update(task);

            await _context.SaveChangesAsync();

            return RedirectToAction("missionDetail", new { Id = taskId });
        }


        public async Task<IActionResult> ApplyForCancle(int taskId)
        {
            // 取得登入的 UserID
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                // 未登入則導向登入頁
                return Json(new { success = false, message = "請先登入再進行操作！", redirect = Url.Action("Login", "Account") });
            }

            int userIdInt = int.Parse(userId);
            // 查詢任務
            var task = await _context.TTasks.FindAsync(taskId);
            if (task == null || task.Fstatus != "進行中")
            {
                //return NotFound(); // 任務不存在或狀態不符時回傳 404
                return Json(new { success = false, message = "任務不存在或狀態不符！" });
            }

            // 檢查是否已有待確認的申請記錄
            bool alreadyApplied = _context.TConfirmReplys.Any(cr =>
                cr.FtaskId == taskId &&
                cr.FworkerId == userIdInt &&
                cr.FconfirmationStatus == "待確認");
            if (alreadyApplied)
            {
                return Json(new { success = false, message = "您已送出申請，請等待發案者回覆！" });
                //return BadRequest("您已送出申請，請等待發案者回覆！");
            }

            // 建立確認回覆記錄，等待發案者同意
            var confirmReply = new TConfirmReply
            {
                FtaskId = taskId,
                FposterId = task.FposterId,   // 發案者ID
                FworkerId = userIdInt,          // 申請者ID
                FconfirmationType = "取消確認",
                FconfirmationStatus = "待確認",
                Fremarks = "等待發案者確認申請"
            };

            _context.TConfirmReplys.Add(confirmReply);
            await _context.SaveChangesAsync();

            // 可額外進行通知發案者（例如寄送通知信或系統訊息）的處理
            return Json(new { success = true, message = "申請取消成功，等待發案者回覆！" });
            //return RedirectToAction("missionDetail", new { Id = taskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmCancleApplication(int taskId)
        {
            // 取得目前登入的發案者 UserID
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            int posterId = int.Parse(userId);

            // 查詢任務，並確認該任務屬於目前發案者
            var task = await _context.TTasks.FindAsync(taskId);
            if (task == null || task.FposterId != posterId)
            {
                return NotFound("任務不存在或您無權操作");
            }

            // 從 tConfirmReplys 取得對應該任務且狀態為「待確認」的申請記錄
            var confirmReply = await _context.TConfirmReplys.FirstOrDefaultAsync(cr =>
                cr.FtaskId == taskId &&

                cr.FconfirmationStatus == "待確認");
            if (confirmReply == null)
            {
                return NotFound("找不到待確認的申請記錄");
            }

            // 從申請記錄中取得接案者 ID
            int workerId = confirmReply.FworkerId;

            // 更新申請記錄狀態，表示發案者已確認該申請
            confirmReply.FconfirmationStatus = "已確認";
            _context.TConfirmReplys.Update(confirmReply);

            // 建立交易記錄
            var transaction = new Ttransaction
            {
                TaskId = taskId,              // 對應 TTransaction 的 Task_ID
                PostUserId = posterId,        // 發案者 ID
                WorkUserId = workerId,        // 從申請記錄中取得的接案者 ID
                Amount = task.Fbudget,        // 任務金額
                Status = "已取消",            // 初始交易狀態
                StartTime = DateTime.UtcNow,  // 交易開始時間
                FinishTime = task.Fdeadline,  // 預計完成時間
                Rating = null,                // 尚無評分
                Review = null                 // 尚無評論
            };
            _context.Ttransactions.Add(transaction);

            // 更新任務狀態，反映目前進入雙方確認階段
            task.Fstatus = "已取消";
            _context.TTasks.Update(task);

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "您已將任務取消" });
            return RedirectToAction("missionDetail", new { Id = taskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTaskAndPay(int taskId)
        {
            // 取得目前登入的 UserID
            var userId = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            int currentUserId = int.Parse(userId);

            // 取得任務資料，並確認該任務存在
            var task = await _context.TTasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound("任務不存在");
            }

            // 驗證目前登入使用者是否為該任務的發案者
            if (task.FposterId != currentUserId)
            {
                return Forbid("您無權執行此操作");
            }

            // 更新任務狀態為「已完成」
            task.Fstatus = "已完成";
            _context.TTasks.Update(task);

            // 根據 TaskId 取得對應的交易記錄，更新交易狀態為「已完成」
            var transaction = await _context.Ttransactions.FirstOrDefaultAsync(t => t.TaskId == taskId);
            if (transaction != null)
            {
                transaction.Status = "已完成";
                _context.Ttransactions.Update(transaction);
            }
            else
            {
                // 若找不到交易記錄，可依需求進行提示或其他處理
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("missionDetail", new { Id = taskId });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(int transactionId, int rating, string review)
        {
            if (rating < 1 || rating > 5)
            {
                ModelState.AddModelError("Rating", "評分必須在 1 到 5 之間");
                return BadRequest(ModelState);
            }

            var transaction = await _context.Ttransactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return NotFound("交易不存在");
            }

            // 更新交易的評價資訊
            transaction.Rating = rating;
            transaction.Review = review;
            await _context.SaveChangesAsync();

            return RedirectToAction("missionList2");
        }


    }
}
