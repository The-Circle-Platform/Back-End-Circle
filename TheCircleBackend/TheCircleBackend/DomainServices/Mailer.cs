using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace TheCircleBackend.DomainServices
{
    public class Mailer
    {

        private readonly IConfiguration _configuration;
        public Mailer(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public void SendMail(string recepient, string subject, string body, string displayName)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp-mail.outlook.com";
            client.Port = 587;
            NetworkCredential nc = new NetworkCredential();
            nc.UserName = _configuration["EmailAddr"];
            nc.Password = _configuration["EmailPswd"];
            client.Credentials = nc;
            client.EnableSsl = true;
            MailAddress from = new MailAddress(_configuration["EmailAddr"], displayName);
            MailAddress receive = new MailAddress(recepient);
            MailMessage message = new MailMessage(from, receive);
            message.Subject = subject;
            message.Body = body;
            client.Send(message);
        }
    }
}
