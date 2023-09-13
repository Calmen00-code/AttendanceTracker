using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceTrackerInfrastructure.Models
{
    public class WorkdayRecordConfiguration : IEntityTypeConfiguration<WorkdayRecord>
    {
        public void Configure(EntityTypeBuilder<WorkdayRecord> builder) 
        {
            builder.ToTable("WorkdayRecords");

            // builder.HasKey(w => w.Date); // Primary Key for Workday is Date

            // Composite Primary Key for Workday is Date and StaffName
            builder.HasKey(w => new { w.Date, w.StaffName }); 

            // currently, we are not implementing StaffName to be primary key,
            // so it allows the same Staff to have multiple different date,
            // but does not allow more than one staff to have the same date
            // Alex can occupied 11/09/2023 and 12/09/2023, but when Amir
            // wanted to acquire 11/09/2023 or 12/09/2023, then it failed

            // if this was implemented, then we can allow for multiple staff having the same day
            // but does not allow a staff to have more than one day record
            // builder.HasKey(w => w.StaffName); // Primary Key for Workday is Date

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

            // builder.HasIndex(w => w.Id); // New index for Id
        }
    }
}
