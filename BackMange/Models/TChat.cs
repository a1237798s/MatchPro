using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TChat
{
    public int FChatId { get; set; }

    public int FPosterId { get; set; }

    public int FWorkerId { get; set; }

    public DateTime? FCreatedAt { get; set; }

    public virtual TUser FPoster { get; set; } = null!;

    public virtual TUser FWorker { get; set; } = null!;

    public virtual ICollection<TMessagesHistory> TMessagesHistories { get; set; } = new List<TMessagesHistory>();
}
