using Core.Models;

namespace Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailModel emailModel);
}