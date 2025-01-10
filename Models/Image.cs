using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class Image
{
    public int Id { get; set; }

    public int? ObjectId { get; set; }

    public string? ObjectType { get; set; }

    public string? ImageUrl { get; set; }

    public virtual LivingObject? Object { get; set; }
}
