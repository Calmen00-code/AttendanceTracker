using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceTrackerInfrastructure.Models
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder) 
        {
            // Table Name
            builder.ToTable("Staffs");

            // Primary Key
            builder.HasKey(x => x.Name);

            // Name is required and maximum length is 100
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Password is required
            builder.Property(s => s.Password)
                .IsRequired();

            // Department is optional
            builder.Property(s => s.Department)
                .HasMaxLength(50);

            // Defining relationship with WorkdayRecord
            // One staff can have more than one WorkdayRecord to keep track 
            // the check in and check out time everyday
            builder.HasMany(s => s.Workdays)
                .WithOne(w => w.Staff)
                .HasForeignKey(w => w.StaffName)
                .OnDelete(DeleteBehavior.Restrict); // Modify this as per your deletion strategy

            // DO NOT DELETE, may be needed in future

            // .WithOne(w => w.Staff)
            // .HasForeignKey(w => w.StaffId)
            // .OnDelete(DeleteBehavior.Restrict); // Modify this as per your deletion strategy

        }
    }
}
