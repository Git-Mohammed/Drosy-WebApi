namespace Drosy.Domain.Entities
{
    public class Country : BaseEntity<int>
    {
        public string Name { get; set; } = null!;

        public List<City> Cities { get; set; } = new();
    }

}