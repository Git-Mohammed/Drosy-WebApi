using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Drosy.Domain.Entities;

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


            builder.HasOne(x => x.AppUser)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(true);
        }
    }
}