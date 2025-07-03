namespace Drosy.Domain.Entities
{
    public class Assistant : BaseEntity<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int UserId { get; set; }
        #region Nav Properties
        public AppUser AppUser = new();
        #endregion

    }
}