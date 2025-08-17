using Drosy.Domain.Entities;
using Drosy.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal partial class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SecondName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ThirdName)
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.EmergencyNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasQueryFilter(x => x.IsDeleted == false);  

            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.DeletedAt).IsRequired(false);
            builder.Property(x => x.DeletedBy).IsRequired(false);

            builder.HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .IsRequired();

            builder.HasOne<ApplicationUser>()
                   .WithOne(x => x.Student)
                   .HasForeignKey<Student>(x => x.UserId)
                   .IsRequired(false);

        }

    }
}