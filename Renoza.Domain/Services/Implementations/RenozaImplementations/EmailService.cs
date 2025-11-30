using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Renoza.Domain.Options;
using Renoza.Domain.Services.Implementations.BaseImplementations;
using Renoza.Domain.Services.Interfaces.RenozaInterfaces;
using Renoza.Infrastructure.Contexts;

namespace Renoza.Domain.Services.Implementations.RenozaImplementations
{
    /// <summary>
    /// Сервис для отправки email
    /// </summary>
    public class EmailService : BaseService<RenozaContext>, IEmailService
    {
        private readonly EmailOptions _emailOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            RenozaContext dbContext,
            IOptions<EmailOptions> emailOptions,
            ILogger<EmailService> logger) : base(dbContext)
        {
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Отправить email
        /// </summary>
        public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailOptions.FromName, _emailOptions.FromEmail));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.Timeout = _emailOptions.TimeoutMs;

                await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.SmtpPort, _emailOptions.UseSsl, cancellationToken);
                await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation("Email успешно отправлен на {To} с темой '{Subject}'", to, subject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке email на {To} с темой '{Subject}'", to, subject);
                return false;
            }
        }

        /// <summary>
        /// Отправить email с кодом верификации
        /// </summary>
        public async Task<bool> SendVerificationCodeAsync(string to, string code, CancellationToken cancellationToken = default)
        {
            var subject = "Код верификации Renoza";
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .container {{
            background-color: #f9f9f9;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
        }}
        .code-container {{
            background-color: #fff;
            border: 2px solid #007bff;
            border-radius: 8px;
            padding: 20px;
            text-align: center;
            margin: 20px 0;
        }}
        .code {{
            font-size: 32px;
            font-weight: bold;
            letter-spacing: 8px;
            color: #007bff;
            font-family: 'Courier New', monospace;
        }}
        .footer {{
            margin-top: 30px;
            text-align: center;
            font-size: 12px;
            color: #666;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Добро пожаловать в Renoza!</h1>
        </div>
        <p>Здравствуйте!</p>
        <p>Вы получили это письмо, потому что кто-то запросил верификацию email адреса в системе Renoza.</p>
        <p>Ваш код верификации:</p>
        <div class=""code-container"">
            <div class=""code"">{code}</div>
        </div>
        <p>Этот код действителен в течение <strong>15 минут</strong>.</p>
        <p>Если вы не запрашивали этот код, просто проигнорируйте это письмо.</p>
        <div class=""footer"">
            <p>С уважением,<br>Команда Renoza</p>
            <p>Это автоматическое сообщение, пожалуйста, не отвечайте на него.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(to, subject, htmlBody, cancellationToken);
        }
    }
}
