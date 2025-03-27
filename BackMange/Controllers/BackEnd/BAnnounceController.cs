using BackMange.DTO;
using BackMange.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Controllers.BackEnd
{
    [Authorize(Roles = "Admin")]
    public class BAnnounceController : Controller
    {
        private readonly GameCaseContext _context;

        public BAnnounceController(GameCaseContext context)
        {
            _context = context;
        }

        //抓資料庫公告類別
        private async Task<SelectList> GetCategoriesAsync()
        {
            var categories = await _context.TAnnounceCategories
                .Select(c => new AnnounceCategoryDTO
                {
                    FCategoryId = c.FCategoryId,
                    FCategoryName = c.FCategoryName
                }).ToListAsync();

            return new SelectList(categories, "FCategoryId", "FCategoryName");
        }

        public IActionResult List()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            var categories = await _context.TAnnounceCategories
             .Select(c => new AnnounceCategoryDTO
             {
                 FCategoryId = c.FCategoryId,
                 FCategoryName = c.FCategoryName
             }).ToListAsync();

            ViewBag.Categories = new SelectList(categories, "FCategoryId", "FCategoryName");
            //設定預設值
            var model = new AnnounceDTO
            {
                FCategoryId = 0,
                FTitle = "",
                FContent = "",
                FCreatedAt = DateTime.Now,
                FUpdatedAt = DateTime.Now,
                Status = "發布",
                FPriority = 1,
            };


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AnnounceDTO DTO, string action)
        {
            // 判斷是草稿還是發布
            var status = DTO.Status = action == "draft" ? "草稿" : "發布";

            if (!ModelState.IsValid)
            {
                TempData["error"] = "公告新增失敗";
                ViewBag.Categories = await GetCategoriesAsync();
                return View(DTO);
            }

            // 創建公告
            var announce = new TAnnounce
            {
                FTitle = DTO.FTitle,
                FContent = DTO.FContent,
                FCategoryId = DTO.FCategoryId,
                FPriority = DTO.FPriority,
                FCreatedAt = DateTime.Now,
                FUpdatedAt = DateTime.Now,
                Status = status,
            };

            _context.TAnnounces.Add(announce);
            await _context.SaveChangesAsync();

            TempData["success"] = "公告新增成功";
            return RedirectToAction("List");
        }

        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromForm] AnnounceCategoryDTO DTO)
        {
            var category = new TAnnounceCategory
            {
                FCategoryName = DTO.FCategoryName
            };

            if (ModelState.IsValid)
            {
                _context.TAnnounceCategories.Add(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "公告新增成功";
                return View();
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var announce = await _context.TAnnounces.FindAsync(id);
            if (announce == null)
            {
                TempData["error"] = "刪除失敗，此公告不存在";
                return RedirectToAction("List");
            }
            if (ModelState.IsValid)
            {
                _context.TAnnounces.Remove(announce);
                await _context.SaveChangesAsync();
                TempData["success"] = "刪除成功";
                return RedirectToAction("List");
            }
            return View();
        }

        //下架文章
        [HttpGet]
        public async Task<IActionResult> Removed(int? id)
        {
            TAnnounce announce = await _context.TAnnounces.FindAsync(id);

            if (announce == null)
            {
                TempData["error"] = "此公告不存在";
                return View();
            }
            announce.Status = "下架";
            announce.FUpdatedAt = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "下架成功";
                    return RedirectToAction("List");
                }
                TempData["error"] = "無法成功下架";
                return RedirectToAction("List");

            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "更新失敗，請稍後再試");
                return View();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Republish(int? id)
        {
            TAnnounce announce = await _context.TAnnounces.FindAsync(id);

            if (announce == null)
            {
                TempData["error"] = "此公告不存在";
                return View();
            }
            announce.Status = "發布";
            announce.FUpdatedAt = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "重新發布成功";
                    return RedirectToAction("List");
                }
                TempData["error"] = "無法成功下架";
                return RedirectToAction("List");

            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "更新失敗，請稍後再試");
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var Announce = await _context.TAnnounces.FindAsync(id);

            if (id == null)
            {
                return View("無此資料");
            }

            var categories = await _context.TAnnounceCategories
             .Select(c => new AnnounceCategoryDTO
             {
                 FCategoryId = c.FCategoryId,
                 FCategoryName = c.FCategoryName
             }).ToListAsync();

            ViewBag.Categories = new SelectList(categories, "FCategoryId", "FCategoryName");

            AnnounceDTO dto = new AnnounceDTO
            {
                FAnnounceId = Announce.FAnnounceId,
                FTitle = Announce.FTitle,
                FContent = Announce.FContent,
                FCategoryId = Announce.FCategoryId,
                FPriority = Announce.FPriority,
                Status = Announce.Status,
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AnnounceDTO dto)
        {
            var announce = await _context.TAnnounces.FindAsync(id);

            if (announce == null)
            {
                TempData["error"] = "此公告不存在";
                return RedirectToAction("List");
            }

            announce.FTitle = dto.FTitle;
            announce.FContent = dto.FContent;
            announce.FCategoryId = dto.FCategoryId;
            announce.FPriority = dto.FPriority;
            announce.FUpdatedAt = DateTime.Now;
            announce.Status = dto.Status;
            try
            {
                if (ModelState.IsValid)
                {
                    await _context.SaveChangesAsync();
                    TempData["success"] = "編輯成功";
                    return RedirectToAction("List");
                }
                return View();

            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "更新失敗，請稍後再試");
                return View(dto);
            }
        }
    }
}
