using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Services;

namespace RepositoryLayer.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(Mail mail);
    }
}
