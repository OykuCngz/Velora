using Microsoft.Extensions.Logging;
using Velora.Application.Common.Interfaces;

namespace Velora.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        // Gerçek dünyada burada MailKit veya SendGrid kullanılır.
        // Biz profesyonelce Serilog ile loglayarak test ediyoruz.
        _logger.LogInformation("E-POSTA GÖNDERİLDİ: \nAlıcı: {To}\nKonu: {Subject}\nİçerik: {Body}", to, subject, body);
        
        return Task.CompletedTask;
    }
}
