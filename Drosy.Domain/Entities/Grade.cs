namespace Drosy.Domain.Entities
{
    public class Grade : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        #region Nav Properties
        public ICollection<Student> Students { get; set; } = new List<Student>();
        #endregion
    }
}