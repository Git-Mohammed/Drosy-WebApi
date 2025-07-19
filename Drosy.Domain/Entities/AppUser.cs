namespace Drosy.Domain.Entities
{
    public class AppUser : BaseEntity<int>
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }
    
}