using BusinessLayer.Interfaces;
using Fundoo.CommonLayer;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
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
            if(existingUser != null && user.Password.Equals(user.ConfirmPassword))
            {
                var updatedUser = userRL.UpdateUser(user);
                return updatedUser;
            }
            return null;
        }

        public bool DeleteUser(int id)
        {
            User user = userRL.GetUser(id);
            if (user != null)
            {
                return userRL.DeleteUser(user);
            }
            return false;
        }

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

        public string ForgotPassword(string email)
        {
            var user = userRL.ForgotPassword(email);
            if (user != null)
                return DecodePassword(user.Password);
            return null;
        }

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

        private string EncodePassword(string password)
        {
            byte[] encData = new byte[password.Length];
            encData = Encoding.UTF8.GetBytes(password);
            string encodedData = Convert.ToBase64String(encData);
            return encodedData;
        }

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
    }
}
