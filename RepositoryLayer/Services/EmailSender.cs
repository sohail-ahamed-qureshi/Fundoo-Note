using CommonLayer;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Services
{
    public class EmailSender : IEmailSender
    {
        private EmailConfiguration emailConfig;
        public EmailSender(EmailConfiguration emailConfiguration)
        {
            emailConfig = emailConfiguration;
        }
        /// <summary>
        /// ability to create email with recipient address, subject and content.
        /// send email with smtp connection and sender's 
        /// </summary>
        /// <param name="mail">details of recipient</param>
        public void SendEmail(Mail mail)
        {
            var emailMessage = CreateMailMessage(mail);

            Send(emailMessage);
        }

        /// <summary>
        /// configuring sender's settings with smtp sever.
        /// </summary>
        /// <param name="emailMessage"></param>
        private void Send(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(emailConfig.SmtpServer, emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailConfig.Username, emailConfig.Password);

                    client.Send(emailMessage);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
        /// <summary>
        /// initializing the recipient's e-mail 
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        private MimeMessage CreateMailMessage(Mail mail)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailConfig.From));
            emailMessage.To.AddRange(mail.To);
            emailMessage.Subject = mail.Subject;
            // .Text - content in text format, .Html - content to be sent in html format 
            emailMessage.Body = new TextPart(TextFormat.Html)
            { Text = string.Format($"<a href='{mail.Content}' style ='color:red'>Reset Password</a>") };

            return emailMessage;
        }
    }
}
