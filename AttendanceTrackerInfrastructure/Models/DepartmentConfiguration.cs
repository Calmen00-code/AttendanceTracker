using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AttendanceTrackerInfrastructure.Models
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder) 
        {
            // Table Name
            builder.ToTable("Departments");

            // Primary Key
            builder.HasKey(d => d.Name);

            // Name is required and maximum length is 50
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
