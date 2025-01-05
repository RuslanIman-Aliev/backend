using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Examin_backend.Models;

public partial class BookingContext : DbContext
{
    public BookingContext()
    {
    }

    public BookingContext(DbContextOptions<BookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Availability> Availabilities { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<FlatInfo> FlatInfos { get; set; }

    public virtual DbSet<HostelInfo> HostelInfos { get; set; }

    public virtual DbSet<HotelInfo> HotelInfos { get; set; }

    public virtual DbSet<HouseInfo> HouseInfos { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Liked> Likeds { get; set; }

    public virtual DbSet<LivingObject> LivingObjects { get; set; }

    public virtual DbSet<ObjectAddress> ObjectAddresses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Special> Specials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserObj> UserObjs { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserTemp> UserTemps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Ruslan;Database=BookingDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Availabi__3214EC07C501A921");

            entity.ToTable("Availability");

            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Object).WithMany(p => p.Availabilities)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Availability_ObjectId");

            entity.HasOne(d => d.User).WithMany(p => p.Availabilities)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Availability_UserId");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC076DB41B65");

            entity.ToTable("Booking");

            entity.Property(e => e.ObjectType).HasMaxLength(50);

            //entity.HasOne(d => d.Availability).WithMany(p => p.Bookings)
            //    .HasForeignKey(d => d.AvailabilityId)
            //    .HasConstraintName("FK_Booking_AvailabilityId");

            entity.HasOne(d => d.Object).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Booking_ObjectId");

            entity.HasOne(d => d.Owner).WithMany(p => p.BookingOwners)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Booking_OwnerId");

            entity.HasOne(d => d.User).WithMany(p => p.BookingUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Booking_UserId");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC079979F2EA");

            entity.ToTable("Cart");

            entity.HasOne(d => d.Object).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Cart_ObjectId");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Cart_UserId");
        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chat__3214EC072E89DC91");

            entity.ToTable("Chat");

            entity.Property(e => e.MessageDate).HasColumnType("datetime");

            entity.HasOne(d => d.Object).WithMany(p => p.Chats)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Chat_ObjectId");

            entity.HasOne(d => d.UserIdFromNavigation).WithMany(p => p.ChatUserIdFromNavigations)
                .HasForeignKey(d => d.UserIdFrom)
                .HasConstraintName("FK_Chat_UserIdFrom");

            entity.HasOne(d => d.UserIdToNavigation).WithMany(p => p.ChatUserIdToNavigations)
                .HasForeignKey(d => d.UserIdTo)
                .HasConstraintName("FK_Chat_UserIdTo");
        });

        modelBuilder.Entity<FlatInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FlatInfo__3214EC07AA8A3E5E");

            entity.ToTable("FlatInfo");

            entity.Property(e => e.DoorCode).HasMaxLength(50);
            entity.Property(e => e.FlatNumber).HasMaxLength(20);
            entity.Property(e => e.HowToGetKey).HasMaxLength(100);
            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Object).WithMany(p => p.FlatInfos)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_FlatInfo_ObjectId");
        });

        modelBuilder.Entity<HostelInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HostelIn__3214EC07551F5B43");

            entity.ToTable("HostelInfo");

            entity.Property(e => e.ForWho).HasMaxLength(50);
            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Object).WithMany(p => p.HostelInfos)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_HostelInfo_ObjectId");
        });

        modelBuilder.Entity<HotelInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HotelInf__3214EC079540CF47");

            entity.ToTable("HotelInfo");

            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Object).WithMany(p => p.HotelInfos)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_HotelInfo_ObjectId");
        });

        modelBuilder.Entity<HouseInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HouseInf__3214EC075E4B9DA4");

            entity.ToTable("HouseInfo");

            entity.Property(e => e.HowToGetKey).HasMaxLength(100);
            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Object).WithMany(p => p.HouseInfos)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_HouseInfo_ObjectId");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3214EC077004A815");

            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Object).WithMany(p => p.Images)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Images_ObjectId");
        });

        modelBuilder.Entity<Liked>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Liked__3214EC07FEA592A6");

            entity.ToTable("Liked");

            entity.HasOne(d => d.Object).WithMany(p => p.Likeds)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Liked_ObjectId");

            entity.HasOne(d => d.User).WithMany(p => p.Likeds)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Liked_UserId");
        });

        modelBuilder.Entity<LivingObject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Object__3214EC07FF0CC83D");

            entity.ToTable("LivingObject");

            entity.Property(e => e.ContactEmail).HasMaxLength(100);
            entity.Property(e => e.ContactNumber).HasMaxLength(25);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Address).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Object_AddressId");

            entity.HasOne(d => d.Availability).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.AvailabilityId)
                .HasConstraintName("FK_Object_AvailabilityId");

            entity.HasOne(d => d.Booking).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Object_BookingId");

            entity.HasOne(d => d.Flat).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.FlatId)
                .HasConstraintName("FK_Object_FlatId");

            entity.HasOne(d => d.Hostel).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.HostelId)
                .HasConstraintName("FK_Object_HostelId");

            entity.HasOne(d => d.Hotel).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.HotelId)
                .HasConstraintName("FK_Object_HotelId");

            entity.HasOne(d => d.House).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.HouseId)
                .HasConstraintName("FK_Object_HouseId");

            entity.HasOne(d => d.Image).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.ImageId)
                .HasConstraintName("FK_Object_ImageId");

            entity.HasOne(d => d.Owner).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Object_OwnerId");

            entity.HasOne(d => d.Reviews).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.ReviewsId)
                .HasConstraintName("FK_Object_ReviewsId");

            entity.HasOne(d => d.Special).WithMany(p => p.LivingObjects)
                .HasForeignKey(d => d.SpecialId)
                .HasConstraintName("FK_Object_SpecialId");
        });

        modelBuilder.Entity<ObjectAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ObjectAd__3214EC0738D1C8CE");

            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.ObjectType).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(100);

            entity.HasOne(d => d.Object).WithMany(p => p.ObjectAddresses)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_ObjectAddresses_ObjectId");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07618926DF");

            entity.HasOne(d => d.FromUser).WithMany(p => p.PaymentFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .HasConstraintName("FK_Payments_FromUserId");

            entity.HasOne(d => d.Object).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Payments_ObjectId");

            entity.HasOne(d => d.ToUser).WithMany(p => p.PaymentToUsers)
                .HasForeignKey(d => d.ToUserId)
                .HasConstraintName("FK_Payments_ToUserId");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC0766C1D39E");

            entity.Property(e => e.ObjectType).HasMaxLength(50);

            entity.HasOne(d => d.Object).WithMany(p => p.ReviewsNavigation)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Reviews_ObjectId");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Reviews_UserId");
        });

        modelBuilder.Entity<Special>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Special__3214EC07A33BDE8A");

            entity.ToTable("Special");

            entity.Property(e => e.ObjectType).HasMaxLength(50);
            entity.Property(e => e.ParkingInfo).HasMaxLength(100);
            entity.Property(e => e.RoomType).HasMaxLength(50);

            entity.HasOne(d => d.Address).WithMany(p => p.Specials)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_Special_AddressId");

            entity.HasOne(d => d.Object).WithMany(p => p.Specials)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_Special_ObjectId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07625ED190");

            entity.ToTable("User");

            entity.HasIndex(e => e.Phone, "UQ__User__5C7E359E2B95F625").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534328EA451").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.LoginDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Surname).HasMaxLength(50);
            entity.Property(e => e.WalletAddress).HasMaxLength(100);
        });

        modelBuilder.Entity<UserObj>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_obj__3214EC07FB196255");

            entity.ToTable("User_obj");

            entity.HasOne(d => d.Object).WithMany(p => p.UserObjs)
                .HasForeignKey(d => d.ObjectId)
                .HasConstraintName("FK_User_obj_ObjectId");

            entity.HasOne(d => d.User).WithMany(p => p.UserObjs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_User_obj_UserId");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_rol__3214EC075FB85CA3");

            entity.ToTable("User_role");

            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_User_role_UserId");
        });

        modelBuilder.Entity<UserTemp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_tem__3214EC07EAF460BA");

            entity.ToTable("User_temp");

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.LoginDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Surname).HasMaxLength(100);
            entity.Property(e => e.WalletAddress).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
