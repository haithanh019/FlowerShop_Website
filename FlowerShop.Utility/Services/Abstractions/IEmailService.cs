namespace FlowerShop.Utility;

/// <summary>
/// Interface for sending emails.
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
}