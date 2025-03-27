using BackMange.Models;
using System.ComponentModel;

namespace BackMange.DTO
{
    public class AnnounceCategoryDTO
    {
        [DisplayName("類別")]
        public int? FCategoryId { get; set; }

        [DisplayName("類別")]
        public string? FCategoryName { get; set; }

    }
}
