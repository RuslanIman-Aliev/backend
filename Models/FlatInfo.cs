using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class FlatInfo
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public string? ObjectType { get; set; }

    public bool? IsBalcon { get; set; }

    public string? DoorCode { get; set; }

    public string? HowToGetKey { get; set; }

    public string? FlatNumber { get; set; }

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual LivingObject? Object { get; set; }
}
