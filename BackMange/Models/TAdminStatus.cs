using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TAdminStatus
{
    public byte FstatusId { get; set; }

    public string FstatusName { get; set; } = null!;

    public string? Fdescription { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public virtual ICollection<TAdmin> TAdmins { get; set; } = new List<TAdmin>();
}
