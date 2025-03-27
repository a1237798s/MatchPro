using BackMange.DTO;
using BackMange.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace BackMange.Controllers.FrontEnd
{
    public class FollowController : Controller
    {
        private readonly GameCaseContext _context;

        public FollowController(GameCaseContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Follow(int followingId, string followingType)
        {
            try
            {
                // 檢查參數
                if (string.IsNullOrEmpty(followingType))
                {
                    return Json(new { success = false, message = "追蹤類型不能為空" });
                }

                var userIdString = HttpContext.Session.GetString("UserID");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Json(new { success = false, message = "請先登入" });
                }

                // 直接比較大寫
                if (followingType == "Worker")  // 改這裡
                {
                    // 檢查是否已追蹤工作者
                    var existingWorkerFollow = await _context.TWorkerFollows
                        .FirstOrDefaultAsync(f =>
                            f.FfollowerId == userId &&
                            f.FworkerUserId == followingId);

                    if (existingWorkerFollow != null)
                    {
                        return Json(new { success = false, message = "已經追蹤過此工作者" });
                    }

                    // 新增工作者追蹤
                    var workerFollow = new TWorkerFollow
                    {
                        FfollowerId = userId,
                        FworkerUserId = followingId,
                        FcreatedAt = DateTime.Now
                    };

                    _context.TWorkerFollows.Add(workerFollow);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "追蹤成功" });
                }

                return Json(new { success = false, message = "無效的追蹤類型" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"追蹤失敗: {ex.Message}");
                return Json(new { success = false, message = "追蹤失敗，請稍後再試" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow(int followingId, string followingType)
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserID");
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Json(new { success = false, message = "請先登入" });
                }

                if (followingType == "Worker")
                {
                    var workerFollow = await _context.TWorkerFollows
                        .FirstOrDefaultAsync(f =>
                            f.FfollowerId == userId &&
                            f.FworkerUserId == followingId);

                    if (workerFollow == null)
                    {
                        return Json(new { success = false, message = "找不到追蹤記錄" });
                    }

                    _context.TWorkerFollows.Remove(workerFollow);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "取消追蹤成功" });
                }

                return Json(new { success = false, message = "無效的追蹤類型" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "取消追蹤失敗，請稍後再試" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> FollowList()
        {
            var userIdString = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var workerFollows = await _context.TWorkerFollows
                .Where(f => f.FfollowerId == userId)
                .Select(f => new WorkerFollowDTO
                {
                    FFollowID = f.FfollowId,
                    FFollowerID = f.FfollowerId,
                    FWorkerUserID = f.FworkerUserId,
                    FCreatedAt = f.FcreatedAt ?? DateTime.Now
                })
                .ToListAsync();

            var taskFollows = await _context.TTaskFollows
                .Where(f => f.FfollowerId == userId)
                .Select(f => new TaskFollowDTO
                {
                    FFollowID = f.FfollowId,
                    FFollowerID = f.FfollowerId,
                    FTaskID = f.FtaskId,
                    FCreatedAt = f.FcreatedAt ?? DateTime.Now
                })
                .ToListAsync();

            return View(new { WorkerFollows = workerFollows, TaskFollows = taskFollows });
        }

        [HttpGet]
        public async Task<IActionResult> CheckFollowStatus(int followingId, string followingType)
        {
            try
            {
                bool isFollowing = false;
                int followCount = 0;

                // 先取得總追蹤數（不管用戶是否登入）
                if (followingType == "Worker")
                {
                    followCount = await _context.TWorkerFollows
                        .CountAsync(f => f.FworkerUserId == followingId);

                    // 只有在用戶已登入時才檢查是否追蹤
                    var userIdString = HttpContext.Session.GetString("UserID");
                    if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
                    {
                        isFollowing = await _context.TWorkerFollows
                            .AnyAsync(f => f.FfollowerId == userId && f.FworkerUserId == followingId);
                    }
                }

                return Json(new { isFollowing, followCount });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"檢查追蹤狀態失敗: {ex.Message}");
                return Json(new { isFollowing = false, followCount = 0 });
            }
        }
        //查看追蹤

        [HttpGet]

        public async Task<IActionResult> GetUserFollowings()
        {
            var userIdString = HttpContext.Session.GetString("UserID");  // 改成 GetString 和 "UserID"
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "請先登入" });
            }

            var followings = await _context.TWorkerFollows
                .Where(f => f.FfollowerId == userId)  // 使用解析後的 userId
                .Select(f => new
                {
                    f.FworkerUserId,
                    CodeName = _context.TWorkers
                        .Where(w => w.FuserId == f.FworkerUserId)
                        .Select(w => w.FcodeName)
                        .FirstOrDefault(),
                    ProfileImage = _context.TImages
                        .Where(img => img.FuserId == f.FworkerUserId &&
                                    img.Fcategory == "profile")
                        .Select(img => img.FimagePath)
                        .FirstOrDefault() ?? "imgs/default-avatar.jpg"
                })
                .ToListAsync();

            return Json(followings);
        }
    }

}