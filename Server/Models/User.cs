using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? ImageUrl { get; set; }

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BorrowingTicket> BorrowingTicketLibrarians { get; set; } = new List<BorrowingTicket>();

    public virtual ICollection<BorrowingTicket> BorrowingTicketMembers { get; set; } = new List<BorrowingTicket>();

    public virtual Member? Member { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role Role { get; set; } = null!;
}
