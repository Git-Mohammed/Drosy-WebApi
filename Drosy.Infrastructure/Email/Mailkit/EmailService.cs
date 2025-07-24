using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drosy.Application.Interfaces.Common;
using Drosy.Application.UseCases.Email.DTOs;
using Drosy.Application.UseCases.Email.Interfaces;
using Drosy.Application.UsesCases.Authentication.DTOs;
using Drosy.Domain.Shared.ApplicationResults;
using Drosy.Domain.Shared.ErrorComponents.Common;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Drosy.Infrastructure.Email.Mailkit
{
    public class EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger) : IEmailService
    {
        private readonly ILogger<EmailService> _logger = logger;
        private readonly EmailOptions _emailOptions = emailOptions.Value;
        public async Task<Result> SendEmailAsync(EmailMessageDTO email, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Aramm Teach", _emailOptions.SenderEmail));
                message.To.Add(new MailboxAddress(email.RecipientName, email.RecipientEmail));
                message.Subject = email.Subject;
                message.Body = new TextPart("html") { Text = email.Body };

                using var client = new MailKit.Net.Smtp.SmtpClient();
                await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailOptions.SenderEmail, _emailOptions.AppPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return Result.Success();
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(CommonErrors.OperationCancelled.Message, "Operation Canceld While sending email");
                return Result.Failure(CommonErrors.OperationCancelled);
            }
            catch (Exception ex)
            {
                return Result.Failure(CommonErrors.Failure);
            }

        }
    }
}
