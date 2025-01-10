using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class HotelInfo
{
    public int Id { get; set; }

    public bool? IsTransferInclude { get; set; }

    public int? RoomNumber { get; set; }

    public int? ObjectId { get; set; }

    public virtual LivingObject? Object { get; set; }
}
