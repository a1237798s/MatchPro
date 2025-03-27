using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TUser
{
    public int FuserId { get; set; }

    public string FfullName { get; set; } = null!;

    public string FidNumber { get; set; } = null!;

    public string? FcompanyNumber { get; set; }

    public string Femail { get; set; } = null!;

    public string? FpasswordHash { get; set; }

    public string? FphoneNumber { get; set; }

    public string? Faddress { get; set; }

    public DateOnly? Fbirthday { get; set; }

    public string? Fgender { get; set; }

    public string? FprofileImageUrl { get; set; }

    public DateTime? FlastLoginTime { get; set; }

    public bool? FisEmailVerified { get; set; }

    public byte? Fstatus { get; set; }

    public string? FsuspensionReason { get; set; }

    public DateTime? FsuspensionEndTime { get; set; }

    public DateTime? FcreatedAt { get; set; }

    public DateTime? FupdatedAt { get; set; }

    public string? FgoogleId { get; set; }

    public string FloginType { get; set; } = null!;

    public virtual ICollection<TChat> TChatFPosters { get; set; } = new List<TChat>();

    public virtual ICollection<TChat> TChatFWorkers { get; set; } = new List<TChat>();

    public virtual ICollection<TEcpayOrder> TEcpayOrderFposters { get; set; } = new List<TEcpayOrder>();

    public virtual ICollection<TEcpayOrder> TEcpayOrderFworkers { get; set; } = new List<TEcpayOrder>();

    public virtual ICollection<TImage> TImages { get; set; } = new List<TImage>();

    public virtual ICollection<TMessagesHistory> TMessagesHistories { get; set; } = new List<TMessagesHistory>();

    public virtual TPoster? TPoster { get; set; }

    public virtual ICollection<TTaskFollow> TTaskFollows { get; set; } = new List<TTaskFollow>();

    public virtual ICollection<TUserNotification> TUserNotificationFSenders { get; set; } = new List<TUserNotification>();

    public virtual ICollection<TUserNotification> TUserNotificationFUsers { get; set; } = new List<TUserNotification>();

    public virtual ICollection<TVerification> TVerifications { get; set; } = new List<TVerification>();

    public virtual TWorker? TWorker { get; set; }

    public virtual ICollection<TWorkerFollow> TWorkerFollowFfollowers { get; set; } = new List<TWorkerFollow>();

    public virtual ICollection<TWorkerFollow> TWorkerFollowFworkerUsers { get; set; } = new List<TWorkerFollow>();

    public virtual ICollection<Ttransaction> TtransactionPostUsers { get; set; } = new List<Ttransaction>();

    public virtual ICollection<Ttransaction> TtransactionWorkUsers { get; set; } = new List<Ttransaction>();

    public virtual ICollection<TUserType> FuserTypes { get; set; } = new List<TUserType>();
}
