using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TConfirmReply
{
    public int FconfirmReplyId { get; set; }

    public int FtaskId { get; set; }

    public int FposterId { get; set; }

    public int FworkerId { get; set; }

    public string FconfirmationType { get; set; } = null!;

    public string FconfirmationStatus { get; set; } = null!;

    public string? Fremarks { get; set; }

    public virtual TPoster Fposter { get; set; } = null!;

    public virtual TTask Ftask { get; set; } = null!;

    public virtual TWorker Fworker { get; set; } = null!;
}
