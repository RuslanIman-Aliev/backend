using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class UserTemp
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? WalletAddress { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime LoginDate { get; set; }

    public DateOnly BirthDate { get; set; }
}
