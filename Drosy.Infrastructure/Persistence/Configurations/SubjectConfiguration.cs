using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Drosy.Domain.Entities;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
       
            builder.HasData(LoadSubjectes());
        }

        private List<Subject> LoadSubjectes()
        {
            return new List<Subject>() {
                new Subject { Id = 1, Name = "رياضيات" },
                new Subject { Id = 2, Name = "فيزياء" },
                new Subject { Id = 3, Name = "كيمياء" }
            };
        }
    }
}