using Microsoft.AspNetCore.Http;

namespace Drosy.Application.UseCases.SystemSettings.DTOs
{
    public class UpdateSystemSettingDTO
    {
        public string WebName { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "USD";
        public IFormFile? LogoFile { get; set; }
    }
}
