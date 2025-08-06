namespace Drosy.Application.UseCases.SystemSettings.DTOs
{
    public class SystemSettingDTO
    {
        public string WebName { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "USD";
        public string? LogoPath { get; set; }
    }
}
