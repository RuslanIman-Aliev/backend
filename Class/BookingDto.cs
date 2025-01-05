namespace Examin_backend.Class
{
    public class BookingDto
    {
        public int ObjectId { get; set; }
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public string ObjectType { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public double TotalSum { get; set; }
        public int Days { get; set; }
        public int Night { get; set; }
        public string Hash { get; set; }
        public string PaymentMethod { get; set; }
        public int Guest { get; set; }
    }

}
