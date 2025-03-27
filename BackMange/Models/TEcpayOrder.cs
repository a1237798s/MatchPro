using System;
using System.Collections.Generic;

namespace BackMange.Models;

public partial class TEcpayOrder
{
    public string MerchantTradeNo { get; set; } = null!;

    public string? MemberId { get; set; }

    public int? RtnCode { get; set; }

    public string? RtnMsg { get; set; }

    public string? TradeNo { get; set; }

    public int? TradeAmt { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentType { get; set; }

    public string? PaymentTypeChargeFee { get; set; }

    public string? TradeDate { get; set; }

    public int? SimulatePaid { get; set; }

    public int? FtaskId { get; set; }

    public int? FposterId { get; set; }

    public int? FworkerId { get; set; }

    public virtual TUser? Fposter { get; set; }

    public virtual TTask? Ftask { get; set; }

    public virtual TUser? Fworker { get; set; }
}
