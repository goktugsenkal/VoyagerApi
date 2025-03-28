using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Core.Interfaces;
using Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly byte[] _secretKey;

    // Constructor retrieves SMTP settings and the secret key from the configuration.
    public EmailService(IConfiguration config)
    {
        _config = config;
        _smtpHost = _config["Smtp:Host"];
        _smtpPort = int.Parse(_config["Smtp:Port"]);          
        _smtpUsername = _config["Smtp:Username"];             
        _smtpPassword = _config["Smtp:Password"];             
        _secretKey = Encoding.UTF8.GetBytes(_config["SecretKey"]);
    }

    // Sends an email using MailKit and MimeKit.
    public async Task SendEmailAsync(EmailModel emailModel)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(emailModel.From));
        email.To.Add(MailboxAddress.Parse(emailModel.To));
        email.Subject = emailModel.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = emailModel.Body };

        using var smtp = new SmtpClient();
        // Connect to the SMTP server using configuration details.
        await smtp.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_smtpUsername, _smtpPassword);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}