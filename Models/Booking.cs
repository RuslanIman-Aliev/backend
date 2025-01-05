using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public int? UserId { get; set; }

    public int? OwnerId { get; set; }

    public string? ObjectType { get; set; }

    public DateOnly? DateIn { get; set; }

    public TimeOnly? TimeIn { get; set; }

    public DateOnly? DateOut { get; set; }

    public TimeOnly? TimeOut { get; set; }

    public int? TotalDayCount { get; set; }

    public int? TotalNightCount { get; set; }

    public double? TotalPayingSum { get; set; }

    public int? Guests { get; set; }

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual LivingObject? Object { get; set; }

    public virtual User? Owner { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
