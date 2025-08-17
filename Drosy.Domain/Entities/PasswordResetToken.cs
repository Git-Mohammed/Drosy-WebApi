namespace Drosy.Domain.Entities
{
    public class PasswordResetToken : BaseEntity<int>
    {
        public string TokenString { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsUsed { get; set; } = false;
        public AppUser User { get; set; } = null!;
    }
}