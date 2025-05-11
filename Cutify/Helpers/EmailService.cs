using System;
using System.Net.Mail;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;

namespace Cutify.Helpers
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Send(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSettings:From").Value));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            var pr = _configuration.GetSection("EmailSettings:Provider").Value;
            smtp.Connect(_configuration.GetSection("EmailSettings:Provider").Value, Convert.ToInt32(_configuration.GetSection("EmailSettings:Port").Value), true);
            smtp.Authenticate(_configuration.GetSection("EmailSettings:UserName").Value, _configuration.GetSection("EmailSettings:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}

