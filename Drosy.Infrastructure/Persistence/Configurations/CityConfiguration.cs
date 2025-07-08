using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Drosy.Domain.Entities;

namespace Drosy.Infrastructure.Persistence.Configurations
{
    internal class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

          

            builder.HasData(LoadCities());
        }

        private List<City> LoadCities()
        {
            return new List<City>() {
                new City { Id = 1, Name = "صنعاء", CountryId = 1 },
                new City { Id = 2, Name = "الرياض", CountryId = 2 }
            };
        }
    }
    
}