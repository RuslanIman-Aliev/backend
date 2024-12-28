using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class Chat
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public int? UserIdFrom { get; set; }

    public int? UserIdTo { get; set; }

    public string? Message { get; set; }

    public DateTime? MessageDate { get; set; }

    public virtual LivingObject? Object { get; set; }

    public virtual User? UserIdFromNavigation { get; set; }

    public virtual User? UserIdToNavigation { get; set; }
}
