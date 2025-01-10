using Microsoft.EntityFrameworkCore;

namespace Examin_backend.Class
{
    public class NewObject
    {
        public string? City { get; set; }
        public string? ListingType { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
        public string? FlatNumber { get; set; }
        public int? OwnerId { get; set; }
        public int? Floor { get; set; }
        public int? MaxCapacity { get; set; }
        public string? Name { get; set; }
        public string? Parking { get; set; }
        public string? PostalCode { get; set; }
        public double? Price { get; set; }
        public int? RoomCount { get; set; }
        public int? Square { get; set; }
        public string? Street { get; set; }
        public int? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public int? PeopleInRoom { get; set; }
        public string? ForWho {  get; set; }
        public string? StreetNumber { get; set; }
        public int? ToiletCount { get; set; }
        public string? Eat { get; set; }
        public string? IsParking {  set; get; }
        public string? IsTransfer { set; get; }
        public string? Balcon {  get; set; }
        public string? Pool {  get; set; }
        public string? Garage { get; set; }
        public string? DoorCode { get; set; }
        public string? GetKey { get; set; }
        public int? FloorCount {  get; set; }
        public List<string>? ImageUrls { get; set; }
        public List<BookObject>? Book { get; set; }

    }



    public class BookObject
    {
        public string? DateIn { get; set; }
        public string? DateOut { get; set; }
    }
}
