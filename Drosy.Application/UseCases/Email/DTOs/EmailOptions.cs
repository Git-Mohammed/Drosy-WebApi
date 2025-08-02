namespace Drosy.Application.UseCases.Email.DTOs
{
    public class EmailOptions
    {
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = null!;
        public string AppPassword { get; set; } = null!;
    }
}
