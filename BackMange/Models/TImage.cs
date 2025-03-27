using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TImage
{
    public int FimageId { get; set; }

    public int FuserId { get; set; }

    public string Frole { get; set; } = null!;

    public string Fcategory { get; set; } = null!;

    public string FimageName { get; set; } = null!;

    public string FimagePath { get; set; } = null!;

    public bool? FisMain { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public virtual TUser Fuser { get; set; } = null!;
}
