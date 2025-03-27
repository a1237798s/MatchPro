using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TWorker
{
    public int FuserId { get; set; }

    public string FcodeName { get; set; } = null!;

    public string? Fskills { get; set; }

    public int? FexperienceYears { get; set; }

    public string? FprofileDescription { get; set; }

    public string? FaboutMe { get; set; }

    public string? FwebsiteUrl { get; set; }

    public string? FserviceAvailability { get; set; }

    public int? FcompletedTasksCount { get; set; }

    public double? Frating { get; set; }

    public bool? FisVerified { get; set; }

    public bool? FisDeleted { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public DateTime? FupdatedAt { get; set; }

    public virtual TUser Fuser { get; set; } = null!;

    public virtual ICollection<TConfirmReply> TConfirmReplies { get; set; } = new List<TConfirmReply>();

    public virtual ICollection<TPortfolio> TPortfolios { get; set; } = new List<TPortfolio>();
}
