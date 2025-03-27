using BackMange.Controllers.API;
using BackMange.DTO;
using BackMange.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace BackMange.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestController : Controller
    {
        private readonly GameCaseContext _context;
        private readonly INotificationService _notificationService;

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }

        public QuestController(GameCaseContext context, INotificationService notificationService)
        {
            _context = context;
            //通知用
            _notificationService = notificationService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> List(string searchQuery, int pageNumber = 1)
        {
            int pageSize = 15; // 每頁顯示 15 筆
            var query = _context.TTasks.AsQueryable(); // 取得查詢物件

            // 如果有輸入搜尋關鍵字，則篩選標題或描述符合的案件
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(t => t.Ftitle.Contains(searchQuery) || t.Fdescription.Contains(searchQuery));
            }

            // **排序邏輯**
            query = query
                .OrderByDescending(t => t.Fstatus == "未審核") // **讓 Fstatus = "未審核" 的案件排在最上面**
                .ThenByDescending(t => t.FcreatedAt); // **依創建時間降序排列**

            // 計算符合條件的總數量
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
                    FupdatedAt = t.FupdatedAt
                })
                .ToListAsync();

            // 傳遞變數到 ViewBag
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchQuery = searchQuery;

            return View(tasks);
        }





        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            // 查詢指定任務的 DTO
            var task = await _context.TTasks
                .Where(s => s.FtaskId == id)
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
                    Fdeadline = td.Fdeadline,
                    FcreatedAt = td.FcreatedAt,
                    FupdatedAt = td.FupdatedAt,
                    FtimagePath = td.Ftimage // 圖片路徑
                })
                .FirstOrDefaultAsync();

            // 如果找不到任務，返回 404 錯誤
            if (task == null)
            {
                return NotFound();
            }

            // 僅允許「待審核」的任務進入審核流程
            if (task.Fstatus != "待審核")
            {
                return BadRequest("此案件已審核，無法修改");
            }

            return View(task); // 傳遞 DTO 給視圖
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            // 查詢指定任務
            var task = await _context.TTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // 僅允許「待審核」的任務進行審核
            if (task.Fstatus != "待審核")
            {
                return BadRequest("此案件已審核");
            }

            // 更新狀態
            task.Fstatus = "發佈中";
            task.FupdatedAt = DateTime.Now;
            _context.TTasks.Update(task);
            await _context.SaveChangesAsync();
            TempData["success"] = "任務新增成功";


            //此處為發送通知
            #region === 發送通知 ===
            // 準備通知：檢查是否已發送過付款通知
            var existingNotification = await _context.TUserNotifications
                .FirstOrDefaultAsync(n =>
                    n.FUserId == task.FposterId &&
                    n.FRelatedId == task.FtaskId &&
                    n.FNotifyType == "Case" &&
                    n.FMessage.Contains("已審核通過"));
            
            if (existingNotification == null)
            {
                // 發送通知給接案者
                var notification = new NotiObj
                {
                    UserId = task.FposterId,  // 接案者ID
                    Content = $"案件「{task.Ftitle}」的已審核通過，並發佈於平台。",
                    Type = "Case",              // 使用 Payment 類型
                    RelatedId = task.FtaskId,           // 相關案件ID
                    SenderId = null,     // 案主ID
                };

                var (success, message, _) = await _notificationService.SendNotificationAsync(notification);

                if (!success) Console.WriteLine($"狀態更新成功，但通知發送失敗：{message}");
                else Console.WriteLine("已通知發案者案件已通過!");
            }
            #endregion === 發送通知 END===

            
            return RedirectToAction("List");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            // 查詢指定任務
            var task = await _context.TTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // 僅允許「未審核」的任務進行退回
            if (task.Fstatus != "待審核")
            {
                return BadRequest("此案件已審核，無法退回");
            }

            _context.TTasks.Remove(task);
            await _context.SaveChangesAsync();
            TempData["success"] = "任務已成功退回";
            return RedirectToAction("List");
        }




        public IActionResult Create()
        {
            return View(); // 返回空白表單頁面
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TTaskDTO dto)
        {
            List<string> imagePaths = new List<string>();

            // 檢查是否有上傳文件
            if (dto.FtimageFiles != null && dto.FtimageFiles.Count > 0)
            {
                foreach (var imageFile in dto.FtimageFiles)
                {
                    // 為每個文件生成唯一名稱
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    // 保存文件到伺服器
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // 添加圖片路徑到列表
                    imagePaths.Add("/uploads/" + fileName);
                }
            }

            // 將圖片路徑保存到資料庫 (這裡可以是逗號分隔、JSON、或一對多關係)
            var task = new TTask
            {
                FtaskId = dto.FtaskId,
                FposterId = dto.FposterId,
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
                Ftimage = string.Join(";", imagePaths) // 多圖片路徑以分號保存到一個字段
            };

            _context.TTasks.Add(task);
            await _context.SaveChangesAsync();
            TempData["success"] = "任務新增成功";
            return RedirectToAction("List");
        }




        public async Task<IActionResult> Detail(int Id)
        {
            var result = await _context.TTasks.Where(s => s.FtaskId == Id).FirstOrDefaultAsync();
          
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
                Fdeadline = td.Fdeadline,
                FcreatedAt = td.FcreatedAt,
                FupdatedAt = td.FupdatedAt,
                FtimagePath = td.Ftimage // 將資料庫中的圖片路徑賦值給 FtimagePath
            })
            .FirstOrDefault();

            if (task == null)
            {
                return NotFound();
            }

            // 獲取對應的訂單
            var order = await _context.TEcpayOrders
                .FirstOrDefaultAsync(o => o.FtaskId == Id);

            if (order != null)
            {
                ViewBag.OrderId = order.MerchantTradeNo; // 設置訂單編號
            }

            return View(task);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("任務 ID 不能為空");
            }

            // 查詢 TTask 實體
            var task = await _context.TTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // 將 TTask 轉換為 TTaskDTO
            var dto = new TTaskDTO
            {
                FtaskId = task.FtaskId,
                FposterId = task.FposterId,
                FcategoryId = task.FcategoryId,
                Ftitle = task.Ftitle,
                Fdescription = task.Fdescription,
                Fbudget = task.Fbudget,
                Flocation = task.Flocation,
                FlocationDetail = task.FlocationDetail,
                Fmember = task.Fmember,
                Fphone = task.Fphone,
                Femail = task.Femail,
                Fstatus = task.Fstatus,
                Fdeadline = task.Fdeadline,
                FcreatedAt = task.FcreatedAt,
                FupdatedAt = task.FupdatedAt,
                FtimagePath = task.Ftimage // 將資料庫中的圖片路徑賦值給 FtimagePath
            };
            
            return View(dto); // 返回刪除確認頁面
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 查詢任務
            var task = await _context.TTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            try
            {
                // 檢查是否有多張圖片
                if (!string.IsNullOrEmpty(task.Ftimage))
                {
                    var imagePaths = task.Ftimage.Split(';'); // 解析分號分隔的圖片

                    foreach (var imagePath in imagePaths)
                    {
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            var fullImagePath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                imagePath.TrimStart('/')
                            );

                            if (System.IO.File.Exists(fullImagePath))
                            {
                                System.IO.File.Delete(fullImagePath); // 刪除圖片文件
                            }
                        }
                    }

                    // 清空圖片路徑
                    task.Ftimage = null;
                }

                // 軟刪除：更新狀態
                task.Fstatus = "已刪除";
                task.FupdatedAt = DateTime.Now;

                // 更新任務
                _context.TTasks.Update(task);
                await _context.SaveChangesAsync();

                TempData["success"] = "任務及相關圖片已成功刪除";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"刪除失敗：{ex.Message}";
                return RedirectToAction("List");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var task = await _context.TTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // 將 TTask 映射到 DTO
            var taskDetailDto = new TTaskDTO(task);

            // 添加狀態列表到 ViewBag
            ViewBag.StatusList = new List<string>
                {
                    "待審核",
                    "發布中",
                    "進行中",
                    "已完成",
                    "已刪除"
                };

            return View(taskDetailDto); // 傳遞 DTO 給 View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, TTaskDTO taskDto)
        {
            try
            {
                var task = await _context.TTasks.FindAsync(id);
                if (task == null)
                {
                    TempData["error"] = "找不到指定的任務";
                    return NotFound();
                }

                // 更新欄位
                task.FposterId = taskDto.FposterId;
                task.FcategoryId = taskDto.FcategoryId;
                task.Ftitle = taskDto.Ftitle;
                task.Fdescription = taskDto.Fdescription;
                task.Fbudget = taskDto.Fbudget;
                task.Flocation = taskDto.Flocation;
                task.FlocationDetail = taskDto.FlocationDetail;
                task.Fmember = taskDto.Fmember;
                task.Fphone = taskDto.Fphone;
                task.Femail = taskDto.Femail;
                task.Fstatus = taskDto.Fstatus;

                // 日期處理
                task.Fdeadline = taskDto.Fdeadline;  // 因為不是可空類型，直接賦值
                task.FupdatedAt = DateTime.Now;

                // 圖片處理
                if (taskDto.FtimageFiles != null && taskDto.FtimageFiles.Count > 0)
                {
                    List<string> imagePaths = new List<string>();
                    foreach (var imageFile in taskDto.FtimageFiles)
                    {
                        if (imageFile.Length > 0)
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

                    // 如果有新上傳的圖片，更新圖片路徑
                    if (imagePaths.Any())
                    {
                        task.Ftimage = string.Join(";", imagePaths);
                    }
                }
                // 如果有傳入圖片路徑但沒有新上傳的圖片，保留原有的圖片路徑
                else if (!string.IsNullOrEmpty(taskDto.FtimagePath))
                {
                    task.Ftimage = taskDto.FtimagePath;
                }

                _context.TTasks.Update(task);
                await _context.SaveChangesAsync();

                TempData["success"] = "任務更新成功";
                return RedirectToAction(nameof(List));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"更新失敗：{ex.Message}";
                return View(taskDto);
            }
        }
    }
}