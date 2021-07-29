using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Services
{
    /// <summary>
    /// model class to mail the recipient on forgot password 
    /// </summary>
    public class Mail
    {
        //email address of recipient
        public List<MailboxAddress> To { get; set; }
        //subject of email
        public string Subject { get; set; }
        //content of email
        public string Content { get; set; }

        public Mail(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress(x)));
            Subject = subject;
            Content = content;

        }
    }
}
