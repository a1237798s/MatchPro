using BackMange.DTO;
using BackMange.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Controllers.FrontEnd
{
    public class FAnnounceController : Controller
    {
        private readonly GameCaseContext _context;

        public FAnnounceController(GameCaseContext context)
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

        public async Task<IActionResult> Fontend()
        {
            var AnnounceCategory = await _context.TAnnounceCategories.Select(
                c => new AnnounceCategoryDTO
                {
                    FCategoryId = c.FCategoryId,
                    FCategoryName = c.FCategoryName,
                }
            ).ToListAsync();

            return View(AnnounceCategory);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var objAnnounces = await _context.TAnnounces.FindAsync(id);
            var category = await _context.TAnnounceCategories.ToListAsync();
            AnnounceDTO dto = new AnnounceDTO
            {
                FTitle = objAnnounces.FTitle,
                FContent = objAnnounces.FContent,
                FCreatedAt = objAnnounces.FCreatedAt,
            };

            ViewBag.category = category;
            return View(dto);
        }
    }
}
