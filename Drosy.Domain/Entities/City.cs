namespace Drosy.Domain.Entities
{
    public class City : BaseEntity<int>
    {
        public string Name { get; set; } = null!;

        public int CountryId { get; set; }
        public int? RegionId { get; set; }
        public virtual Region? Region { get; set; } 
        public virtual Country Country { get; set; } = null!;
    }

}