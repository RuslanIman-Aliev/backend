using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class HostelInfo
{
    public int Id { get; set; }

    public int? PeopleInRoom { get; set; }

    public string? ForWho { get; set; }

    public int? RoomNumber { get; set; }

    public int? ObjectId { get; set; }

    public virtual LivingObject? Object { get; set; }
}
