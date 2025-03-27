using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TAnnounce
{
    public int FAnnounceId { get; set; }

    public string FTitle { get; set; } = null!;

    public string FContent { get; set; } = null!;

    public int FCategoryId { get; set; }

    public int FPriority { get; set; }

    public DateTime FCreatedAt { get; set; }

    public DateTime FUpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual TAnnounceCategory FCategory { get; set; } = null!;
}
