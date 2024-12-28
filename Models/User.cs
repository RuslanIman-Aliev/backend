using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? WalletAddress { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public DateTime? LoginDate { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

    public virtual ICollection<Booking> BookingOwners { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingUsers { get; set; } = new List<Booking>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Chat> ChatUserIdFromNavigations { get; set; } = new List<Chat>();

    public virtual ICollection<Chat> ChatUserIdToNavigations { get; set; } = new List<Chat>();

    public virtual ICollection<Liked> Likeds { get; set; } = new List<Liked>();

    public virtual ICollection<LivingObject> LivingObjects { get; set; } = new List<LivingObject>();

    public virtual ICollection<Payment> PaymentFromUsers { get; set; } = new List<Payment>();

    public virtual ICollection<Payment> PaymentToUsers { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<UserObj> UserObjs { get; set; } = new List<UserObj>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
