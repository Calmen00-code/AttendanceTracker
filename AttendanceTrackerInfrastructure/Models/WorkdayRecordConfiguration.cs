using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceTrackerInfrastructure.Models
{
    public class WorkdayRecordConfiguration : IEntityTypeConfiguration<WorkdayRecord>
    {
        public void Configure(EntityTypeBuilder<WorkdayRecord> builder) 
        {
            builder.ToTable("WorkdayRecord");

            builder.HasKey(w => w.StaffName); // Workday has no Primary Key

            builder.Property(w => w.Date)
                .IsRequired();

            builder.Property(w => w.CheckIn)
                .IsRequired();

            builder.Property(w => w.CheckOut) 
                .IsRequired();

            // Defining the foreign key relationship with Staff

            builder.HasOne(w => w.Staff) // WorkdayRecord has one staff
                .WithMany(s => s.Workdays) // and that one staff can have a lot of workdays
                .HasForeignKey(w => w.StaffName) // foreign key for Workday to references Staff: StaffName
                .IsRequired() // Foreign Key is required
                .OnDelete(DeleteBehavior.Cascade); // When staff is deleted, respective workday will be deleted too
        }
    }
}
