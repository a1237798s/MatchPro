using BackMange.Models;
using BackMange.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackMange.Controllers.FrontEnd
{
    public class FTaskFollowController : Controller
    {
        private readonly GameCaseContext _context;

        public FTaskFollowController(GameCaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Follow([FromForm] int followingId)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserID");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Json(new { success = false, message = "請先登入" });
                }

                // 檢查是否已追蹤
                var existingFollow = await _context.TTaskFollows
                    .FirstOrDefaultAsync(f => f.FfollowerId == userId && f.FtaskId == followingId);

                if (existingFollow != null)
                {
                    return Json(new { success = false, message = "已經追蹤過此任務" });
                }

                // 新增追蹤
                var taskFollow = new TTaskFollow
                {
                    FfollowerId = userId,
                    FtaskId = followingId,
                    FcreatedAt = DateTime.Now
                };

                _context.TTaskFollows.Add(taskFollow);
                await _context.SaveChangesAsync();

                // 取得最新追蹤數
                var followCount = await _context.TTaskFollows
                    .CountAsync(f => f.FtaskId == followingId);

                return Json(new { success = true, message = "追蹤成功", followCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "追蹤失敗，請稍後再試" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow([FromForm] int followingId)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserID");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Json(new { success = false, message = "請先登入" });
                }

                var taskFollow = await _context.TTaskFollows
                    .FirstOrDefaultAsync(f => f.FfollowerId == userId && f.FtaskId == followingId);

                if (taskFollow == null)
                {
                    return Json(new { success = false, message = "找不到追蹤記錄" });
                }

                _context.TTaskFollows.Remove(taskFollow);
                await _context.SaveChangesAsync();

                // 取得最新追蹤數
                var followCount = await _context.TTaskFollows
                    .CountAsync(f => f.FtaskId == followingId);

                return Json(new { success = true, message = "取消追蹤成功", followCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "取消追蹤失敗，請稍後再試" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckFollowStatus(int followingId)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserID");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Json(new { isFollowing = false, followCount = 0 });
                }

                bool isFollowing = await _context.TTaskFollows
                    .AnyAsync(f => f.FfollowerId == userId && f.FtaskId == followingId);

                var followCount = await _context.TTaskFollows
                    .CountAsync(f => f.FtaskId == followingId);

                return Json(new { isFollowing, followCount });
            }
            catch (Exception ex)
            {
                return Json(new { isFollowing = false, followCount = 0 });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUserTaskFollowings()
        {
            var userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var followings = await _context.TTaskFollows
                .Where(f => f.FfollowerId == userId)
                .Select(f => new
                {
                    f.FtaskId,
                    TaskTitle = f.Ftask.Ftitle,
                    Budget = f.Ftask.Fbudget,
                    Deadline = f.Ftask.Fdeadline,
                   
                })
                .ToListAsync();

            return Json(followings);
        }
    }

}
