using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class HouseInfo
{
    public int Id { get; set; }

    public bool? IsPoolInclude { get; set; }

    public bool? IsGarage { get; set; }

    public string? HowToGetKey { get; set; }

    public int? FloatCount { get; set; }

    public int? ObjectId { get; set; }

    public virtual LivingObject? Object { get; set; }
}
