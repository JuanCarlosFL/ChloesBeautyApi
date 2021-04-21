using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ChloesBeauty.API.Models
{
    public partial class ChloesBeautyContext : DbContext
    {
        #region Public Constructors

        public ChloesBeautyContext()
        {
        }

        public ChloesBeautyContext(DbContextOptions<ChloesBeautyContext> options)
            : base(options)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public virtual DbSet<Appointment> Appointments { get; set; }

        public virtual DbSet<Availability> Availabilities { get; set; }

        public virtual DbSet<Loyalty> Loyalties { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Treatment> Treatments { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UsersRole> UsersRoles { get; set; }

        #endregion Public Properties

        #region Private Methods

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion Private Methods

        #region Protected Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(e => e.AppointmentId).HasColumnName("appointmentID");

                entity.Property(e => e.AvailabilityId).HasColumnName("availabilityID");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");

                entity.Property(e => e.PersonId).HasColumnName("personID");

                entity.Property(e => e.TreatmentId).HasColumnName("treatmentID");

                entity.HasOne(d => d.Availability)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.AvailabilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Treatment)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(d => d.TreatmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Availability>(entity =>
            {
                entity.Property(e => e.AvailabilityId).HasColumnName("availabilityID");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");
            });

            modelBuilder.Entity<Loyalty>(entity =>
            {
                entity.Property(e => e.LoyaltyId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("loyaltyID");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.Property(e => e.TreatmentId).HasColumnName("treatmentID");

                //entity.HasOne(d => d.LoyaltyNavigation)
                //    .WithOne(p => p.Loyalty)
                //    .HasForeignKey<Loyalty>(d => d.LoyaltyId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Loyalties_Treatments_treatmentID");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.PersonId).HasColumnName("personID");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Comments)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("comments");

                entity.Property(e => e.ContactHow)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("contactHow");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.Property(e => e.PostCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("postCode");

                entity.Property(e => e.Surname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("surname");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("telephone");

                entity.Property(e => e.Town)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("town");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("roleID");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Treatment>(entity =>
            {
                entity.Property(e => e.TreatmentId).HasColumnName("treatmentID");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PersonId).HasColumnName("personID");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("userName");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<UsersRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("Users_Roles");

                entity.Property(e => e.UserId).HasColumnName("userID");

                entity.Property(e => e.RoleId).HasColumnName("roleID");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("modifiedDate");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles_Users1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        #endregion Protected Methods
    }
}