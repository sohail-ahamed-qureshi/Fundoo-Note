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

        public string Authenticate(string userEmail)
        {
            string SecretKey = "FundooNotes BridgeLabz by Sohail Ahamed Q";
            var user = GetUser(userEmail);
            if (user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(SecretKey);
                var tokenDescpritor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, user.FirstName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescpritor);
                string jwtToken = tokenHandler.WriteToken(token);
                return jwtToken;
            }
            return null;
        }
    }
}
