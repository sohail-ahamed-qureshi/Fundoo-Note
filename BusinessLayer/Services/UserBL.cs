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
        UserRL userObj = new UserRL();
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
            if (newUser != null)
            {
                User user = userRL.RegisterNewUser(newUser);
                return user;
            }
            return null;

        }

        public User GetUser(int userid)
        {
            if (userid > 0)
            {
                var user = userRL.GetUser(userid);
                return user;
            }
            return null;
        }

        public User UpdateUser(User user)
        {
            if (user != null && userObj.users.Contains(user))
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
                userRL.DeleteUser(user);
                return true;
            }
            return false;
        }

        public User UserLogin(Login login)
        {
            if (login != null)
            {
                var user = userRL.UserLogin(login);
                return user;
            }
            return null;
        }

        public string ForgotPassword(string userName)
        {
            string password = userRL.ForgotPassword(userName);
            return password;
        }

        public User ResetPassword(int userId, string password)
        {
            if (userId > 0)
            {
                var existingUser = userRL.GetUser(userId);
                if (existingUser != null)
                {
                    User updateUser = userRL.ResetPassword(existingUser, password);
                    return updateUser;
                }
            }
            return null;
        }
    }
}
