using Fundoo.CommonLayer;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        public List<User> users = new List<User>()
        {
            new User(){ UserId = 1, UserName ="sohail", age= 25, password = "12345", Occupation = "Engineer" }
        };

        public List<User> GetUsers()
        {
            return users;
        }

        public User RegisterNewUser(User newUser)
        {
            newUser.UserId = users.Count + 1;
            users.Add(newUser);
            return newUser;
        }

        public User GetUser(int userid)
        {
            var user = users.Find(user => user.UserId == userid);
            return user;
        }

        public User UpdateUser(User user)
        {
            User existingUser = GetUser(user.UserId);
            existingUser.UserName = user.UserName;
            existingUser.password = user.password;
            existingUser.age = user.age;
            existingUser.Occupation = user.Occupation;
            return existingUser;
        }

        public void DeleteUser(User user)
        {
            users.Remove(user);
        }

        public User UserLogin(Login login)
        {
            var user = users.Find(x => x.UserName == login.userName && x.password == login.password);
            return user;
        }

        public string ForgotPassword(string userName)
        {
            User existingUser = users.Find(user => user.UserName == userName);
            if (existingUser != null)
                return existingUser.password;
            return null;
        }

        public User ResetPassword(User user, string password)
        {
            user.password = password;
            return user;
        }
    }
}
