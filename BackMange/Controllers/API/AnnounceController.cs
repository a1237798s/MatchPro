using BackMange.DTO;
using BackMange.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Controllers.API
{
    public class AnnounceController : Controller
    {
        private readonly GameCaseContext _context;

        public AnnounceController(GameCaseContext context)
        {
            _context = context;
        }

        /*後臺datatable使用API*/
        [HttpGet]
        public async Task<ActionResult> GetAll(int? categoryId)
        {
            var query = _context.TAnnounces.Include(c => c.FCategory).AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(a => a.FCategory.FCategoryId == categoryId);
            }

            var Announce = await query.Select(c => new AnnounceDTO
            {
                FAnnounceId = c.FAnnounceId,
                FTitle = c.FTitle,
                FContent = c.FContent,
                FCategoryId = c.FCategory.FCategoryId,
                FCategoryName = c.FCategory.FCategoryName,
                FPriority = c.FPriority,
                FCreatedAt = c.FCreatedAt,
                FUpdatedAt = c.FUpdatedAt,
                Status = c.Status,
            }).ToListAsync();

            return Json(new { Data = Announce });
        }

        /*前台datatable使用API*/
        public async Task<ActionResult> GetAllFont(int? categoryId)
        {
            var query = _context.TAnnounces.Include(c => c.FCategory).AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(a => a.FCategory.FCategoryId == categoryId);
            }

            var Announce = await query.Where(a => a.Status == "發布").Select(c => new AnnounceDTO
            {
                FAnnounceId = c.FAnnounceId,
                FTitle = c.FTitle,
                FContent = c.FContent,
                FCategoryId = c.FCategory.FCategoryId,
                FCategoryName = c.FCategory.FCategoryName,
                FPriority = c.FPriority,
                FCreatedAt = c.FCreatedAt,
                FUpdatedAt = c.FUpdatedAt,
                Status = c.Status,
            }).ToListAsync();

            return Json(new { Data = Announce });
        }
    }
}
