using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer
{
    /// <summary>
    /// model class for secret key of jwt token
    /// </summary>
    public class AuthenticationSettings
    {
        public string SecretKey { get; set; }
    }
}
