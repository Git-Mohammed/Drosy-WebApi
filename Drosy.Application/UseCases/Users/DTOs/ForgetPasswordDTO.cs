namespace Drosy.Application.UsesCases.Users.DTOs
{
    public class ForgetPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
