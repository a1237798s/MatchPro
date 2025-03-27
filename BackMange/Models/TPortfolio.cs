using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TPortfolio
{
    public int PortfolioId { get; set; }

    public int FuserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? MediaUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual TWorker Fuser { get; set; } = null!;
}
