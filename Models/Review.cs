using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class Review
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ObjectId { get; set; }

    public string? ObjectType { get; set; }

    public string? Text { get; set; }

    public int? StarsCount { get; set; }

    public bool? IsUserBookedObj { get; set; }

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual LivingObject? Object { get; set; }

    public virtual User? User { get; set; }
}
