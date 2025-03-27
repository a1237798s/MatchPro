using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TWorkerFollow
{
    public int FfollowId { get; set; }

    public int FfollowerId { get; set; }

    public int FworkerUserId { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public virtual TUser Ffollower { get; set; } = null!;

    public virtual TUser FworkerUser { get; set; } = null!;
}
