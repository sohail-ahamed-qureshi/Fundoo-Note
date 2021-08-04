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
        MessageQueue messageQueue = null;
        public UserBL(IUserRL userRL, IEmailSender emailSender, IConfiguration config)
        {
            this.userRL = userRL;
            this.emailSender = emailSender;
            secretKey = config.GetSection("Settings").GetSection("SecretKey").Value;
        }
        /// <summary>
        /// utility method to get all user details from database
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsers()
        {
            return userRL.GetUsers();
        }
        /// <summary>
        /// ability to register new user to database, validating user details and passwords
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
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
        /// <summary>
        /// utility method to get user data using userID.
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
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
        /// <summary>
        /// utility method to get user from database using email property.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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
        public User ResetPassword(int userId, ResetPassword resetPassword)
        {
            if (resetPassword.NewPassword.Equals(resetPassword.ConfirmPassword))
            {
                User existingUser = userRL.GetUser(userId);
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
        /// <summary>
        /// ability to generate jwt token with 10mins of expiry time.
        /// </summary>
        /// <param name="userEmail">userEmail</param>
        /// <param name="userId">user id</param>
        /// <returns>jwt token in string format</returns>
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
        public string ResetEmail(User user)
        {
            string token = Authenticate(user.Email, user.UserId);
            if (token == null)
                return null;
            return token;
        }
        /// <summary>
        /// passing the message to queue
        /// </summary>
        /// <param name="user"></param>
        public void SendMessageQueue(User user)
        {
            string token = ResetEmail(user);
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
                Message message1 = new Message();
                message1.Formatter = new BinaryMessageFormatter();
                messageQueue.ReceiveCompleted += Msmq_RecieveCompleted;
                messageQueue.Label = "url link";
                message1.Body = token;
                messageQueue.Send(message1);
                messageQueue.BeginReceive();
                messageQueue.Close();
                //return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// msmq recieve event handler which sends email asynchronously
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Msmq_RecieveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var message = messageQueue.EndReceive(e.AsyncResult);
            message.Formatter = new BinaryMessageFormatter();
            string token = message.Body.ToString();
            string userEmail = ExtractData(token);
            var emailMessage = new Mail(new string[] { userEmail }, "Fundoo Note - Reset Password", $"https://localhost:44333/api/user/resetpassword/{token} ");
            emailSender.SendEmail(emailMessage);
        }

        /// <summary>
        /// ability to extract data from token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>returns user email</returns>
        public string ExtractData(string token)
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
                string userEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var userId = Convert.ToInt32(principal.Claims.SingleOrDefault(c => c.Type == "userId").Value);
                return userEmail;
            }
            catch
            {
                principal = null;
            }
            return null;
        }
    }
}
