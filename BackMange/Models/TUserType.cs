using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TUserType
{
    public int FuserTypeId { get; set; }

    public string FuserTypeName { get; set; } = null!;

    public virtual ICollection<TUser> Fusers { get; set; } = new List<TUser>();
}
