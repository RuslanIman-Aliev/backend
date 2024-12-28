using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class HouseInfo
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public string? ObjectType { get; set; }

    public bool? IsEatInclude { get; set; }

    public bool? IsPoolInclude { get; set; }

    public bool? IsGarage { get; set; }

    public string? HowToGetKey { get; set; }

    public int? FloorCount { get; set; }

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual LivingObject? Object { get; set; }
}
