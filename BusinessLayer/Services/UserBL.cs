using BusinessLayer.Interfaces;
using Experimental.System.Messaging;
using Fundoo.CommonLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        private IUserRL userRL;
        private readonly IEmailSender emailSender;
        private string secretKey;
        public UserBL(IUserRL userRL, IEmailSender emailSender, IConfiguration config)
        {
            this.userRL = userRL;
            this.emailSender = emailSender;
            secretKey = config.GetSection("Settings").GetSection("SecretKey").Value;
        }

        public List<User> GetUsers()
        {
            return userRL.GetUsers();
        }

        public User UserRegister(User newUser)
        {
            if (newUser != null && newUser.Password.Equals(newUser.ConfirmPassword))
            {
                newUser.Password = EncodePassword(newUser.Password);
                var user = userRL.RegisterNewUser(newUser);
                return user;
            }
            return null;
        }

        public User GetUser(int userid)
        {
            if (userid > 0)
            {
                var user = userRL.GetUser(userid);
                user.Password = DecodePassword(user.Password);
                return user;
            }
            return null;
        }

        public User GetUser(string email)
        {
            if (email != null || string.IsNullOrEmpty(email))
            {
                var user = userRL.GetUser(email);
                return user;
            }
            return null;
        }
        /// <summary>
        /// to update details user has to first login then
        /// update details
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User UpdateUser(User user)
        {
            Login login = new Login();
            login.Email = user.Email;
            login.Password = user.Password;
            var existingUser = UserLogin(login);
            if (existingUser != null && user.Password.Equals(user.ConfirmPassword))
            {
                user.Password = EncodePassword(user.Password);
                user.ConfirmPassword = user.Password;
                var updatedUser = userRL.UpdateUser(user);
                return updatedUser;
            }
            return null;
        }
        /// <summary>
        /// ability to check the user details before letting user delete
        /// </summary>
        /// <param name="id">get details using userid</param>
        /// <returns>true if delete successful</returns>
        public bool DeleteUser(int id)
        {
            User user = userRL.GetUser(id);
            if (user != null)
            {
                return userRL.DeleteUser(user);
            }
            return false;
        }
        /// <summary>
        /// ability to login a user using email and password
        /// and generate token for successfull login
        /// </summary>
        /// <param name="login">email and password</param>
        /// <returns>user details and token</returns>
        public User UserLogin(Login login)
        {
            User user = null;
            if (login != null)
                user = userRL.UserLogin(login);
            if (user != null)
            {
                user.Password = DecodePassword(user.Password);
                return user.Password.Equals(login.Password) ? user : null;
            }
            return null;
        }
        /// <summary>
        /// ability to check whether the user details are present in the database 
        /// and retieve password in decoded format 
        /// </summary>
        /// <param name="email">email of user</param>
        /// <returns>password</returns>
        public User ForgotPassword(string email)
        {
            var user = userRL.ForgotPassword(email);
            if (user != null)
                return user;
            return null;
        }
        /// <summary>
        /// ability to reset password using email and new password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        public User ResetPassword(User existingUser, ResetPassword resetPassword)
        {
            if (resetPassword.NewPassword.Equals(resetPassword.ConfirmPassword))
            {
                resetPassword.NewPassword = EncodePassword(resetPassword.NewPassword);
                User user = userRL.ResetPassword(existingUser, resetPassword.NewPassword);
                return user;
            }
            return null;
        }
        /// <summary>
        /// ability to encrypt password using UTF8 standards
        /// </summary>
        /// <param name="password">user password</param>
        /// <returns>encrypted password</returns>
        private string EncodePassword(string password)
        {
            byte[] encData = new byte[password.Length];
            encData = Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData);
            return encodedData;
        }
        /// <summary>
        /// ability to decrypt password into human readable format
        /// </summary>
        /// <param name="encPassword"></param>
        /// <returns>decrypted password</returns>
        private string DecodePassword(string encPassword)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            Decoder decoder = encoder.GetDecoder();
            byte[] todecodeByte = Convert.FromBase64String(encPassword);
            int charCount = decoder.GetCharCount(todecodeByte, 0, todecodeByte.Length);
            char[] decodeChar = new char[charCount];
            decoder.GetChars(todecodeByte, 0, todecodeByte.Length, decodeChar, 0);
            string password = new String(decodeChar);
            return password;
        }

        public string Authenticate(string userEmail, int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescpritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Email, userEmail),
                        new Claim("userId", userId.ToString(), ClaimValueTypes.Integer),
                    }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescpritor);
            string jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }

        /// <summary>
        /// On forgot pasword 
        /// check for user validation 
        /// generate a token
        /// send that token to user and the api link to reset the password.
        /// receiev forgot password and confirm pssword and 
        /// validate the token and extract email id from it
        /// reset the password of the user in database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool ResetEmail(User user)
        {

            string token = Authenticate(user.Email, user.UserId);

            //sending an email with the token and link to reset password
            var emailMessage = new Mail(new string[] { user.Email }, "Fundoo Note - Reset Password", $"https://localhost:44333/api/user/resetpassword/{token} ");
            emailSender.SendEmail(emailMessage);

            //create msmq to send email to user ;
            SendMessage(token, emailMessage);
            RecieveMessage();

            return true;
        }


        private void SendMessage(string message, object value)
        {
            MessageQueue messageQueue = null;
            string description = message;
            string path = @".\private$\ForgotPassword";

            try
            {
                if (MessageQueue.Exists(path))
                {
                    messageQueue = new MessageQueue(path);
                }
                else
                {
                    MessageQueue.Create(path);
                    messageQueue = new MessageQueue(path);
                }
                string result = message + value;
                messageQueue.Send(result, description);
            }
            catch
            {
                throw;
            }
        }

        private string RecieveMessage()
        {
            MessageQueue myQueue = null;
            string result = null;
            string path = @".\private$\ForgotPassword";

            try
            {
                myQueue = new MessageQueue(path);
                Message[] messages = myQueue.GetAllMessages();
                if (messages.Length > 0)
                {
                    foreach (Message msg in messages)
                    {
                        msg.Formatter = new XmlMessageFormatter(new string[] { "System.String, mscorlib" });
                        result = msg.Body.ToString();
                        myQueue.Receive();
                        File.WriteAllText(@"C:\Users\Admin\Desktop\BridgeLabs Assignments\#WebApplication\Fundoo-Note\Fundoo\RecieveMessages.txt", result);

                    }
                    myQueue.Refresh();
                }
                else
                {
                    Console.WriteLine("No Messages");
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public User ExtractData(string token)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters parameters = new TokenValidationParameters
            {

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false

            };
            SecurityToken securityToken;
            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                var userId = Convert.ToInt32(principal.Claims.SingleOrDefault(c => c.Type == "userId").Value);
                if(userId != 0)
                {
                    User existingUser = GetUser(userId);
                    return existingUser;
                }
            }
            catch
            {
                principal = null;
            }
            return null;
        }
    }
}
