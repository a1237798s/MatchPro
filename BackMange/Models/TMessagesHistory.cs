using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TMessagesHistory
{
    public int FMessageId { get; set; }

    public int FChatId { get; set; }

    public int FSenderId { get; set; }

    public string FMessageText { get; set; } = null!;

    public DateTime FSentAt { get; set; }

    public bool FIsRead { get; set; }

    public virtual TChat FChat { get; set; } = null!;

    public virtual TUser FSender { get; set; } = null!;
}
