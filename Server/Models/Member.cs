using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public int UserId { get; set; }

    public string? IdcardNumber { get; set; }

    public DateOnly MembershipDate { get; set; }

    public DateOnly? MembershipExpiry { get; set; }

    public int MaxBorrowLimit { get; set; }

    public DateTime? SuspendedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
