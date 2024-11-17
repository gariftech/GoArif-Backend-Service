
using SendingEmail;

public interface IEmailService
{
    Task SendEmailAsync(EmailForm model);

}