using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TTask
{
    public int FtaskId { get; set; }

    public int FposterId { get; set; }

    public int FcategoryId { get; set; }

    public string Ftitle { get; set; } = null!;

    public string Fdescription { get; set; } = null!;

    public int Fbudget { get; set; }

    public string Flocation { get; set; } = null!;

    public string FlocationDetail { get; set; } = null!;

    public string Fmember { get; set; } = null!;

    public string Fphone { get; set; } = null!;

    public string Femail { get; set; } = null!;

    public string Fstatus { get; set; } = null!;

    public DateTime Fdeadline { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public DateTime? FupdatedAt { get; set; }

    public string? Ftimage { get; set; }

    public virtual ICollection<TConfirmReply> TConfirmReplies { get; set; } = new List<TConfirmReply>();

    public virtual ICollection<TEcpayOrder> TEcpayOrders { get; set; } = new List<TEcpayOrder>();

    public virtual ICollection<TTaskFollow> TTaskFollows { get; set; } = new List<TTaskFollow>();

    public virtual ICollection<Ttransaction> Ttransactions { get; set; } = new List<Ttransaction>();
}
