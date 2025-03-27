using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TPoster
{
    public int FuserId { get; set; }

    public string? FcompanyName { get; set; }

    public string? FcompanyRegistrationNumber { get; set; }

    public double? FreputationScore { get; set; }

    public bool? FisVerified { get; set; }

    public bool? FisDeleted { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public DateTime? FupdatedAt { get; set; }

    public virtual TUser Fuser { get; set; } = null!;

    public virtual ICollection<TConfirmReply> TConfirmReplies { get; set; } = new List<TConfirmReply>();
}
