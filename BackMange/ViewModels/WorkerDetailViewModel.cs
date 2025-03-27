using BackMange.DTO;

namespace BackMange.ViewModels
{
    public class WorkerDetailViewModel
    {
        public WorkerDTO Worker { get; set; } //存放當前查看的接案者資料

        public List<WorkerDTO> RecommendedWorkers { get; set; } //RecommendedWorkers：存放推薦的其他接案者

        // 評價分頁相關
        public int ReviewPage { get; set; }
        public int TotalReviewPages { get; set; }
        public int ReviewPageSize { get; set; } = 5;  // 每頁顯示數量

        // 接案紀錄分頁相關 //ReviewPage/TransactionPage：記錄當前頁碼
        public int TransactionPage { get; set; }
        public int TotalTransactionPages { get; set; }
        public int TransactionPageSize { get; set; } = 5;  // 每頁顯示數量

        public WorkerDetailViewModel()
        {
            Worker = new WorkerDTO();
            RecommendedWorkers = new List<WorkerDTO>();

            // 初始化分頁相關屬性
            ReviewPage = 1;
            TotalReviewPages = 1;
            TransactionPage = 1;
            TotalTransactionPages = 1;
        }
    }
}
