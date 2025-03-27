using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TTaskFollow
{
    public int FfollowId { get; set; }

    public int FfollowerId { get; set; }

    public int FtaskId { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public virtual TUser Ffollower { get; set; } = null!;

    public virtual TTask Ftask { get; set; } = null!;
}
