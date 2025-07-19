using Drosy.Domain.Entities;
using Drosy.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);


            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);



            builder.Property(x => x.UserId)
                .IsRequired(true);


            builder.HasOne<ApplicationUser>()
                   .WithOne(x => x.Teacher)
                   .HasForeignKey<Teacher>(x => x.UserId)
                   .IsRequired();
        }
    }
}