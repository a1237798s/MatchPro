using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TAnnounceCategory
{
    public int FCategoryId { get; set; }

    public string FCategoryName { get; set; } = null!;

    public virtual ICollection<TAnnounce> TAnnounces { get; set; } = new List<TAnnounce>();
}
