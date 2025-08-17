namespace Drosy.Domain.Entities
{
    public class Region : BaseEntity<int>
    {
        public string Name { get; set; } = null!;

        public virtual ICollection<City> Cities { get; set; } = new HashSet<City>();
    }

}