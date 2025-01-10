using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class LivingObject
{
    public int Id { get; set; }

    public int? AddressId { get; set; }

    public int? OwnerId { get; set; }

    public int? SpecialId { get; set; }

    public string? ObjectType { get; set; }

    public string? Description { get; set; }

    public double? Price { get; set; }

    public string? Name { get; set; }

    public int? Square { get; set; }

    public virtual ObjectAddress? Address { get; set; }

    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<FlatInfo> FlatInfos { get; set; } = new List<FlatInfo>();

    public virtual ICollection<HostelInfo> HostelInfos { get; set; } = new List<HostelInfo>();

    public virtual ICollection<HotelInfo> HotelInfos { get; set; } = new List<HotelInfo>();

    public virtual ICollection<HouseInfo> HouseInfos { get; set; } = new List<HouseInfo>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Liked> Likeds { get; set; } = new List<Liked>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Special? Special { get; set; }

    public virtual ICollection<UserObj> UserObjs { get; set; } = new List<UserObj>();
}
