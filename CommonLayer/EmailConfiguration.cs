using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer
{
    /// <summary>
    /// model class for email configuration defined in appsettings.json
    /// </summary>
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
