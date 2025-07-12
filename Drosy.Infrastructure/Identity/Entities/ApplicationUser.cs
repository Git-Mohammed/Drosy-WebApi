using Drosy.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Drosy.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public List<RefreshToken> RefreshTokens { get; set; }

        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
        public Assistant? Assistant { get; set; }
    }
}