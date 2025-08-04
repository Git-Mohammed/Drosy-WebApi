namespace Drosy.Domain.Entities
{
    public class SystemSetting : BaseEntity<int>
    {
        public string WebName { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "USD";
        public string? LogoPath { get; set; }
    }
}