using System;
using System.Collections.Generic;

namespace Server.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public bool IsRead { get; set; }

    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
