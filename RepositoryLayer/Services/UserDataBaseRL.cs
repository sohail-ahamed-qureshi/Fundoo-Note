using Fundoo.CommonLayer;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Services
{

    public class UserDataBaseRL : IUserRL
    {
        private UserContext userContext;
        public UserDataBaseRL(UserContext userContext)
        {
            this.userContext = userContext;
        }
        public void DeleteUser(User user)
        {
            userContext.Users.Remove(user);
            userContext.SaveChanges();
        }

        public string ForgotPassword(string userName)
        {
            var user = userContext.Users.FirstOrDefault(user=> user.UserName == userName);
            return user.password;
        }

        public User GetUser(int userid)
        {
            var user = userContext.Users.Find(userid);
            return user;
        }

        public List<User> GetUsers()
        {
            return userContext.Users.ToList();
        }

        public User RegisterNewUser(User newUser)
        {
            userContext.Users.Add(newUser);
            userContext.SaveChanges();
            return newUser;
        }

        public User ResetPassword(User user, string password)
        {
            user.password = password;
            userContext.SaveChanges();
            return user;
        }

        public User UpdateUser(User user)
        {
            var existingUser = GetUser(user.UserId);
            existingUser.UserName = user.UserName;
            existingUser.password = user.password;
            existingUser.age = user.age;
            existingUser.Occupation = user.Occupation;
            userContext.SaveChanges();
            return existingUser;
        }

        public User UserLogin(Login login)
        {
            var user = userContext.Users.FirstOrDefault(user => user.UserName == login.userName && user.password == login.password);
            return user;
        }
    }
}
