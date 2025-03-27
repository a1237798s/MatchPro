using BackMange.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackMange.DTO
{
    public class AnnounceDTO
    {
        [DisplayName("公告ID")]
        public int FAnnounceId { get; set; }

        [DisplayName("標題")]
        [Required(ErrorMessage="這是必填欄位!")]
        public string FTitle { get; set; }

        [DisplayName("內容")]
        [Required(ErrorMessage = "這是必填欄位!")]
        public string FContent { get; set; }

        [DisplayName("類別")]
        [Required(ErrorMessage = "請選擇類別!")]
        public int FCategoryId { get; set; }
        [ValidateNever]
        [DisplayName("類別")]
        public string? FCategoryName { get; set; }

        [DisplayName("優先權")]
        public int FPriority { get; set; }
        [ValidateNever]
        public string? PriorityLabel => GetPriorityLabel(FPriority); // 自動轉換

        [DisplayName("發布時間")]
        public DateTime FCreatedAt { get; set; }

        [DisplayName("更新時間")]
        public DateTime FUpdatedAt { get; set; }

        [DisplayName("狀態")]
        [Required(ErrorMessage = "請選擇狀態!")]
        public string Status { get; set; }

        
        private string? GetPriorityLabel(int priority)
        {
            return priority switch
            {
                3 => "置頂",
                2 => "緊急",
                1 => "一般",
                _ => "",
            };
        }
    }
}
