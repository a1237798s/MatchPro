using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TAdmin
{
    public int FadminId { get; set; }

    public string FadminNo { get; set; } = null!;

    public string FfullName { get; set; } = null!;

    public string Femail { get; set; } = null!;

    public string FadmPassword { get; set; } = null!;

    public string? FmobilePhone { get; set; }

    public int FadminLevel { get; set; }

    public byte FstatusId { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public DateTime? FupdatedAt { get; set; }

    public virtual TAdminStatus Fstatus { get; set; } = null!;
}
