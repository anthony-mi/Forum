using Forum.Models.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Forum.Services
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings = null;
        private readonly IHostingEnvironment _env = null;
        private SmtpClient _client = null;

        public MailKitEmailSender(IOptions<EmailSettings> emailSettings, IHostingEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _env = env;
            InitializeSmtpClient();
        }

        private void InitializeSmtpClient()
        {
            _client = new SmtpClient();

            // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            if (_env.IsDevelopment())
            {
                // The third parameter is useSSL (true if the client should make an SSL-wrapped
                // connection to the server; otherwise, false).
                _client.Connect(_emailSettings.MailServer, _emailSettings.MailPort, true);
            }
            else
            {
                _client.Connect(_emailSettings.MailServer);
            }

            // Note: only needed if the SMTP server requires authentication
            _client.Authenticate(_emailSettings.Sender, _emailSettings.Password);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (_client == null) // Not already initialized
            {
                InitializeSmtpClient();
            }

            try
            {
                MimeMessage mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
                mimeMessage.To.Add(new MailboxAddress(email));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };

                await _client.SendAsync(mimeMessage);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }

        ~MailKitEmailSender()
        {
            _client.DisconnectAsync(true);
        }
    }
}
