using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class HotelInfo
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public string? ObjectType { get; set; }

    public bool? IsEatInclude { get; set; }

    public bool? IsTransferInclude { get; set; }

    public int? RoomNumber { get; set; }

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual LivingObject? Object { get; set; }
}
