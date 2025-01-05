using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public int? FromUserId { get; set; }

    public int? ToUserId { get; set; }

    public string? PaymentTransaction { get; set; }

    public string? PaymentMethod { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual LivingObject? Object { get; set; }

    public virtual User? ToUser { get; set; }
}
