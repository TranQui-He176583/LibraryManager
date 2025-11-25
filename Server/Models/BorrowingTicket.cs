using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class BorrowingTicket
{
    public int TicketId { get; set; }

    public int MemberId { get; set; }

    public int LibrarianId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<BorrowingDetail> BorrowingDetails { get; set; } = new List<BorrowingDetail>();

    public virtual User Librarian { get; set; } = null!;

    public virtual User Member { get; set; } = null!;
}
