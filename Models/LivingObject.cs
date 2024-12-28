using System;
using System.Collections.Generic;

namespace Examin_backend.Models;

public partial class LivingObject
{
    public int Id { get; set; }

    public int? AddressId { get; set; }

    public int? OwnerId { get; set; }

    public int? ImageId { get; set; }

    public int? HostelId { get; set; }

    public int? SpecialId { get; set; }

    public int? HouseId { get; set; }

    public int? HotelId { get; set; }

    public int? FlatId { get; set; }

    public int? BookingId { get; set; }

    public int? AvailabilityId { get; set; }

    public bool? PossiblePayInCash { get; set; }

    public string? ObjectType { get; set; }

    public string? Description { get; set; }

    public string? ContactNumber { get; set; }

    public string? ContactEmail { get; set; }

    public int? ReviewsId { get; set; }

    public double? Square { get; set; }

    public double? Price { get; set; }

    public string? Name { get; set; }

    public virtual ObjectAddress? Address { get; set; }

    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

    public virtual Availability? Availability { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual FlatInfo? Flat { get; set; }

    public virtual ICollection<FlatInfo> FlatInfos { get; set; } = new List<FlatInfo>();

    public virtual HostelInfo? Hostel { get; set; }

    public virtual ICollection<HostelInfo> HostelInfos { get; set; } = new List<HostelInfo>();

    public virtual HotelInfo? Hotel { get; set; }

    public virtual ICollection<HotelInfo> HotelInfos { get; set; } = new List<HotelInfo>();

    public virtual HouseInfo? House { get; set; }

    public virtual ICollection<HouseInfo> HouseInfos { get; set; } = new List<HouseInfo>();

    public virtual Image? Image { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Liked> Likeds { get; set; } = new List<Liked>();

    public virtual ICollection<ObjectAddress> ObjectAddresses { get; set; } = new List<ObjectAddress>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Review? Reviews { get; set; }

    public virtual ICollection<Review> ReviewsNavigation { get; set; } = new List<Review>();

    public virtual Special? Special { get; set; }

    public virtual ICollection<Special> Specials { get; set; } = new List<Special>();

    public virtual ICollection<UserObj> UserObjs { get; set; } = new List<UserObj>();
}
