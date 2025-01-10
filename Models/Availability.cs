using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class Availability
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateOnly? DateIn { get; set; }

    public TimeOnly? TimeIn { get; set; }

    public DateOnly? DateOut { get; set; }

    public TimeOnly? TimeOut { get; set; }

    public int? ObjectId { get; set; }

    public virtual LivingObject? Object { get; set; }

    public virtual User? User { get; set; }
}
