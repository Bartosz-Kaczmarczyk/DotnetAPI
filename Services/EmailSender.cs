using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace DotnetAPI.Services;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (SmtpClient client = new SmtpClient("smtp.outlook.com", 587))
        {
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("dotnetapitest@outlook.com", "dotnetAPI");
            client.EnableSsl = true;

            var msg = new MailMessage();
            msg.From = new MailAddress("dotnetapitest@outlook.com");
            msg.To.Add(email);
            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = htmlMessage;  
            
            await client.SendMailAsync(msg);
        }
    }
}