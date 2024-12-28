using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class UserObj
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ObjectId { get; set; }

    public virtual LivingObject? Object { get; set; }

    public virtual User? User { get; set; }
}
