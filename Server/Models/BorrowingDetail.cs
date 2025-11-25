using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class BorrowingDetail
{
    public int DetailId { get; set; }

    public int TicketId { get; set; }

    public int BookId { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Fine> Fines { get; set; } = new List<Fine>();

    public virtual BorrowingTicket Ticket { get; set; } = null!;
}
