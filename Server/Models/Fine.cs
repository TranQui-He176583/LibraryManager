using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Fine
{
    public int FineId { get; set; }

    public int BorrowDetailId { get; set; }

    public decimal Amount { get; set; }

    public string Reason { get; set; } = null!;

    public bool IsPaid { get; set; }

    public DateTime? PaidDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Notes { get; set; }

    public virtual BorrowingDetail BorrowDetail { get; set; } = null!;
}
