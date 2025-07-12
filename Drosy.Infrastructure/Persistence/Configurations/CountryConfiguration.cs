using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Drosy.Domain.Entities;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(x => x.Cities)
           .WithOne(x => x.Country)
           .HasForeignKey(x => x.CountryId)
           .IsRequired();
            builder.HasData(LoadCountries());
        }

        private List<Country> LoadCountries()
        {
            return new List<Country>() {
                new Country { Id = 1, Name = "اليمن" },
                new Country { Id = 2, Name = "السعودية" },
                new Country { Id = 3, Name = "مصر" }
            };
        }
    
    }
}