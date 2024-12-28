using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? RoleName { get; set; }

    public virtual User? User { get; set; }
}
