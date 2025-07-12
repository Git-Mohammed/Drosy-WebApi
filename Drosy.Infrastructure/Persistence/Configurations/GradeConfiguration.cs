using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Drosy.Domain.Entities;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(x => x.Students)
             .WithOne(x => x.Grade)
             .HasForeignKey(x => x.GradeId)
             .IsRequired();


            builder.HasData(LoadGrades());
        }

        private List<Grade> LoadGrades()
        {
            return new List<Grade>() {
                new Grade { Id = 1, Name = "اول ثانوي" },
                new Grade { Id = 2, Name = "ثاني ثانوي" },
                new Grade { Id = 3, Name = "ثالث ثانوي" }
            };
         }  
    }
}