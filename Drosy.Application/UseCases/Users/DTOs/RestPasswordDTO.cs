namespace Drosy.Application.UsesCases.Users.DTOs
{
    public class RestPasswordDTO
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmedPassword { get; set; } = string.Empty;
    }
}
