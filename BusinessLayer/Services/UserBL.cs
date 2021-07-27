using BusinessLayer.Interfaces;
using Fundoo.CommonLayer;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLayer.Services
{
    public class UserBL : IUserBL
    {
        string secretKey = "FundooUser_BridgeLabz";
        static string Jtoken;
        private IUserRL userRL;
        public UserBL(IUserRL userRL)
        {
            this.userRL = userRL;
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
        public string ForgotPassword(string email)
        {
            var user = userRL.ForgotPassword(email);
            if (user != null)
                return DecodePassword(user.Password);
            return null;
        }
        /// <summary>
        /// ability to reset password using email and new password
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        public User ResetPassword(ResetPassword resetPassword)
        {
            if (resetPassword.NewPassword.Equals(resetPassword.ConfirmPassword) && resetPassword.Email != null)
            {
                var existingUser = userRL.GetUser(resetPassword.Email);
                if (existingUser != null)
                {
                    resetPassword.NewPassword = EncodePassword(resetPassword.NewPassword);
                    User updateUser = userRL.ResetPassword(existingUser, resetPassword.NewPassword);
                    return updateUser;
                }
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
        /// ability to generate token for the user with expiring time of one day
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GenerateToken(string userName, string userEmail, int userId)
        {
            var issuer = "http://mysite.com";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
            permClaims.Add(new Claim(ClaimTypes.Name, userName));
            permClaims.Add(new Claim( ClaimTypes.Email, userEmail));
            var token = new JwtSecurityToken(issuer,
                    issuer,
                    permClaims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials);
            Jtoken = new JwtSecurityTokenHandler().WriteToken(token);
            return Jtoken;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null) return null;
                byte[] key = Convert.FromBase64String(Jtoken);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string ValidateToken(string token)
        {
            string email = null;
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null) return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim userEmailClaim = identity.FindFirst(ClaimTypes.Email);
            email = userEmailClaim.Value;
            return email;
        }
    }
}
