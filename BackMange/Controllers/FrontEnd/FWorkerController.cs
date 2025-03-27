using BackMange.DTO;
using BackMange.Models;
using BackMange.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BackMange.Controllers.FrontEnd
{
    public class FWorkerController : Controller
    {
        private readonly GameCaseContext _db;
        private readonly string _connectionString;

        public FWorkerController(GameCaseContext context, IConfiguration configuration)
        {
            _db = context;
            _connectionString = configuration.GetConnectionString("GameCase")
            ?? throw new InvalidOperationException("找不到連線字串 'GameCase'");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> List(int page = 1, int pageSize = 12)
        {
            var query = _db.TWorkers.AsQueryable();
            Console.WriteLine($"查詢結果{query}");
            return await GetPaginatedResult(query, page, pageSize);
        }
        private async Task<List<WorkerDTO>> GetWorkerDTOs(List<TWorker> workers)
        {
            return workers.Select(worker =>
            {
                var profileImage = _db.TUsers
                    .FirstOrDefault(img => img.FuserId == worker.FuserId);
                string profileImagePath = "uploads/Profile/"+ profileImage?.FprofileImageUrl ?? "imgs/default-avatar.jpg";

                var backgroundImages = _db.TImages
                    .Where(img => img.FuserId == worker.FuserId &&
                                 img.Fcategory == "background")
                    .Select(img => img.FimagePath)
                    .ToList();
                // 計算追蹤數
                var followCount = _db.TWorkerFollows
                    .Count(f => f.FworkerUserId == worker.FuserId);

                var dto = new WorkerDTO(worker, profileImagePath, backgroundImages);
                dto.FollowCount = followCount;  // 設定追蹤數
                return dto;
            }).ToList();
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkers(string sortOrder = "default", double? minRating = null, int? minCases = null, string[]? skills = default, int page = 1, int pageSize = 12)
        {
            var query = _db.TWorkers.AsQueryable();
            if (minRating.HasValue)
            {
                query = query.Where(w => w.Frating >= minRating.Value);
            }

            if (minCases.HasValue)
            {
                query = query.Where(w => w.FcompletedTasksCount >= minCases.Value);
            }

            if (skills != null && skills.Length > 0)
            {
                query = query.Where(w => skills.Any(skill =>  // 這裡使用 query
               (skill == "前端" && (w.Fskills.Contains("HTML") || w.Fskills.Contains("JavaScript") || w.Fskills.Contains("React"))) ||
               (skill == "後端" && (w.Fskills.Contains("C#") || w.Fskills.Contains("ASP.NET") || w.Fskills.Contains("SQL"))) ||
               (skill == "UI" && (w.Fskills.Contains("UI") || w.Fskills.Contains("Figma"))) ||
               (skill == "平面" && (w.Fskills.Contains("Photoshop") || w.Fskills.Contains("Illustrator"))) ||
               (skill == "影片" && (w.Fskills.Contains("Premiere") || w.Fskills.Contains("After Effects"))) ||
               (skill == "攝影" && w.Fskills.Contains("攝影")) ||
               (skill == "文案" && w.Fskills.Contains("文案")) ||
               (skill == "社群" && (w.Fskills.Contains("社群") || w.Fskills.Contains("內容行銷")))
              ));
            }

            switch (sortOrder)
            {
                case "1":
                    query = query.OrderByDescending(w => w.Frating);
                    break;
                case "2":
                    query = query.OrderByDescending(w => w.FcompletedTasksCount);
                    break;
                case "3":  // 加入按追蹤數排序
                    query = query.OrderByDescending(w => _db.TWorkerFollows
                        .Count(f => f.FworkerUserId == w.FuserId));
                    break;
                default:
                    query = query.OrderBy(w => w.FuserId);
                    break;
            }

            return await GetPaginatedResult(query, page, pageSize, true);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query, string sortOrder = "default", double? minRating = null, int? minCases = null, string[]? skills = default, int page = 1, int pageSize = 12)
        {
            var searchQuery = _db.TWorkers
                .Where(w => w.FcodeName.Contains(query) ||
                           w.Fskills.Contains(query));

            // 新增：篩選條件
            if (minRating.HasValue)
            {
                searchQuery = searchQuery.Where(w => w.Frating >= minRating.Value);
            }

            if (minCases.HasValue)
            {
                searchQuery = searchQuery.Where(w => w.FcompletedTasksCount >= minCases.Value);
            }

            if (skills != null && skills.Length > 0)
            {
                searchQuery = searchQuery.Where(w => skills.Any(skill =>  // 這裡改用 searchQuery
            (skill == "前端" && (w.Fskills.Contains("HTML") || w.Fskills.Contains("JavaScript") || w.Fskills.Contains("React"))) ||
            (skill == "後端" && (w.Fskills.Contains("C#") || w.Fskills.Contains("ASP.NET") || w.Fskills.Contains("SQL"))) ||
            (skill == "UI" && (w.Fskills.Contains("UI") || w.Fskills.Contains("Figma"))) ||
            (skill == "平面" && (w.Fskills.Contains("Photoshop") || w.Fskills.Contains("Illustrator"))) ||
            (skill == "影片" && (w.Fskills.Contains("Premiere") || w.Fskills.Contains("After Effects"))) ||
            (skill == "攝影" && w.Fskills.Contains("攝影")) ||
            (skill == "文案" && w.Fskills.Contains("文案")) ||
            (skill == "社群" && (w.Fskills.Contains("社群") || w.Fskills.Contains("內容行銷")))
        ));
            }
            // 加入排序邏輯
            switch (sortOrder)
            {
                case "1":
                    searchQuery = searchQuery.OrderByDescending(w => w.Frating);
                    break;
                case "2":
                    searchQuery = searchQuery.OrderByDescending(w => w.FcompletedTasksCount);
                    break;
                case "3":
                    searchQuery = searchQuery.OrderByDescending(w => _db.TWorkerFollows
                        .Count(f => f.FworkerUserId == w.FuserId));
                    break;
                default:
                    searchQuery = searchQuery.OrderBy(w => w.FuserId);
                    break;
            }

            return await GetPaginatedResult(searchQuery, page, pageSize, true);
        }

        // 共用的分頁處理方法
        private async Task<IActionResult> GetPaginatedResult(IQueryable<TWorker> query,
    int page, int pageSize, bool isJson = false)
        {
            // 計算總數
            int totalWorkers = await query.CountAsync();

            // 計算總頁數，確保至少為 1
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)totalWorkers / pageSize));

            // 確保當前頁不超過總頁數
            page = Math.Min(Math.Max(1, page), totalPages);

            // 分頁
            var workers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var workerDTOs = await GetWorkerDTOs(workers);

            var result = new
            {
                Workers = workerDTOs,
                TotalWorkers = totalWorkers,
                PageSize = pageSize,
                currentPage = page,
                totalPages = totalPages,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };

            if (isJson)
                return Json(result);

            ViewBag.TotalWorkers = totalWorkers;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(workerDTOs);
        }

        [HttpGet]
        public IActionResult Details(int id, int reviewPage = 1, int transactionPage = 1)
        {
            var userId = HttpContext.Session.GetString("UserID");

            using GameCaseContext db = new GameCaseContext();

            // 查詢接案者資料，包含交易記錄和任務資料
            var worker = db.TWorkers
                .Include(w => w.Fuser)
                    .ThenInclude(u => u.TtransactionWorkUsers)
                        .ThenInclude(t => t.Task)
                .FirstOrDefault(w => w.FuserId == id);

            if (worker == null)
                return NotFound();

            // 查詢個人資料圖片
            var profileImage = (from user in db.TUsers
                                where user.FuserId == id                               
                                select user).FirstOrDefault();

            // 查詢背景圖片
            var backgroundImages = (from img in db.TImages
                                    where img.FuserId == id
                                    && img.Frole == "worker"
                                    && img.Fcategory == "background"
                                    select img.FimagePath).ToList();

            // 查詢追蹤數
            var followCount = db.TWorkerFollows.Count(f => f.FworkerUserId == id);

            // 建立 WorkerDTO
            var workerDTO = new WorkerDTO(
                worker,
                "uploads/Profile/"+profileImage?.FprofileImageUrl ?? "uploads/default/profile.jpg",
                backgroundImages
            );
            workerDTO.FollowCount = followCount;

            // 計算評價總頁數
            var totalReviews = worker.Fuser?.TtransactionWorkUsers?
                .Count(t => t.Rating.HasValue && !string.IsNullOrEmpty(t.Review)) ?? 0;
            var totalReviewPages = (int)Math.Ceiling(totalReviews / 5.0);

            // 計算已完成案件總頁數
            var totalTransactions = worker.Fuser?.TtransactionWorkUsers?
                .Count(t => t.Status == "已完成") ?? 0;
            var totalTransactionPages = (int)Math.Ceiling(totalTransactions / 5.0);

            // 分頁處理評價
            if (worker.Fuser?.TtransactionWorkUsers != null)
            {
                var pagedReviews = worker.Fuser.TtransactionWorkUsers
                    .Where(t => t.Rating.HasValue && !string.IsNullOrEmpty(t.Review))
                    .Skip((reviewPage - 1) * 5)
                    .Take(5)
                    .ToList();

                // 分頁處理已完成案件
                var pagedTransactions = worker.Fuser.TtransactionWorkUsers
                    .Where(t => t.Status == "已完成")
                    .Skip((transactionPage - 1) * 5)
                    .Take(5)
                    .ToList();

                // 更新 DTO 中的交易記錄
                workerDTO.Transactions = pagedTransactions.Select(t => new WorkerTransactionDTO
                {
                    TransactionId = t.TransactionId,
                    TaskTitle = t.Task?.Ftitle,
                    Amount = t.Amount,
                    Status = t.Status,
                    StartTime = t.StartTime,
                    FinishTime = t.FinishTime,
                    Rating = t.Rating,
                    Review = t.Review
                }).ToList();
            }

            // 取得推薦列表
            var recommendedWorkers = GetRecommendedWorkers(worker, db);

            // 建立 ViewModel
            var viewModel = new WorkerDetailViewModel
            {
                Worker = workerDTO,
                RecommendedWorkers = recommendedWorkers,
                ReviewPage = reviewPage,
                TotalReviewPages = totalReviewPages,
                TransactionPage = transactionPage,
                TotalTransactionPages = totalTransactionPages
            };
            ViewBag.WorkerId = id;  // 將 WorkerId 傳到 View
            ViewBag.UserId = string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
            return View(viewModel);
        }
        [HttpGet]  // 這個方法如果會被其他 action 呼叫，可以不用加 HttpGet
        private List<WorkerDTO> GetRecommendedWorkers(TWorker currentWorker, GameCaseContext db)
        {
            var currentWorkerSkills = currentWorker.Fskills.Split(',')
                .Select(s => s.Trim())
                .ToList();

            return db.TWorkers
                .Where(w => w.FuserId != currentWorker.FuserId)
                .AsEnumerable()
                .Select(w => new
                {
                    Worker = w,
                    MatchingSkills = w.Fskills.Split(',')
                        .Select(s => s.Trim())
                        .Intersect(currentWorkerSkills, StringComparer.OrdinalIgnoreCase)
                        .Count()
                })
                .Where(x => x.MatchingSkills > 0)
                .OrderByDescending(x => x.MatchingSkills)
                .ThenByDescending(x => x.Worker.Frating)
                .Take(4)
                .Select(x => {
                    var dto = new WorkerDTO(
                        x.Worker,
                        db.TImages
                            .Where(img => img.FuserId == x.Worker.FuserId &&
                                   img.Frole == "worker" &&
                                   img.Fcategory == "profile")
                            .Select(img => img.FimagePath)
                            .FirstOrDefault() ?? "uploads/default/profile.jpg",
                        db.TImages
                            .Where(img => img.FuserId == x.Worker.FuserId &&
                                   img.Frole == "worker" &&
                                   img.Fcategory == "background")
                            .Select(img => img.FimagePath)
                            .ToList()
                    );
                    dto.FollowCount = db.TWorkerFollows.Count(f => f.FworkerUserId == dto.FUserId);
                    return dto;
                })
                .ToList();
        }

    }
}
