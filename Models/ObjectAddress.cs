using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class ObjectAddress
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public string? ObjectType { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual LivingObject? Object { get; set; }

    public virtual ICollection<Special> Specials { get; set; } = new List<Special>();
}
