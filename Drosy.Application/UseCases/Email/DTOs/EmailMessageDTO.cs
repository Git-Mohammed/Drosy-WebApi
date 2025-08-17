using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drosy.Application.UseCases.Email.DTOs
{
    public class EmailMessageDTO
    {
        public string RecipientName { get; set; } = null!;
        public string RecipientEmail { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsHtml { get; set; } = true;
    }
}
