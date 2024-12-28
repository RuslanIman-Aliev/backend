using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class Special
{
    public int Id { get; set; }

    public int? RoomCount { get; set; }

    public int? AddressId { get; set; }

    public int? ObjectId { get; set; }

    public string? ObjectType { get; set; }

    public int? MaxPeopleCapacity { get; set; }

    public int? ToiletCount { get; set; }

    public string? ParkingInfo { get; set; }

    public int? Floor { get; set; }

    public int? TotalSquare { get; set; }

    public int? TotalCapacity { get; set; }

    public string? RoomType { get; set; }

    public virtual ObjectAddress? Address { get; set; }

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual LivingObject? Object { get; set; }
}
