using System;
using System.ComponentModel.DataAnnotations;

namespace BackMange.ViewModels
{
    public class WorkerViewModel
    {
        public int? FuserId { get; set; }
        public string? FcodeName { get; set; }
        public string? Fskills { get; set; }
        public int? FexperienceYears { get; set; }
        public string? FprofileDescription { get; set; }
        public string? FwebsiteUrl { get; set; }
        public int? FcompletedTasksCount { get; set; }
        public double? Frating { get; set; }
        public bool? FisVerified { get; set; }
    }

}