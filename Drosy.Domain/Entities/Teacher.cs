namespace Drosy.Domain.Entities
{
    public class Teacher : BaseEntity<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public int UserId { get; set; }
        #region Nav Properties
        public AppUser AppUser = new();
        #endregion

    }
}