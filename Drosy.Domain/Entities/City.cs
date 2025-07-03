namespace Drosy.Domain.Entities
{
    public class City : BaseEntity<int>
    {
        public string Name { get; set; } = null!;

        public int CountryId { get; set; }

       public Country Country { get; set; } = new Country();
    }
}