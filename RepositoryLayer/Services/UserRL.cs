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
            new User(){ UserId = 1, FirstName ="sohail", LastName= "Ahamed", Password = "12345", ConfirmPassword = "12345" ,Email = "sohailqureshi82@gmail.com", CreatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")    }
        };

        public List<User> GetUsers()
        {
            return users;
        }

        public User RegisterNewUser(User newUser)
        {
            newUser.UserId = users.Count + 1;
            newUser.CreatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            newUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            users.Add(newUser);
            return newUser;
        }

        //not required
        public User GetUser(int userid)
        {
            var user = users.Find(user => user.UserId == userid);
            return user;
        }

        public User GetUser(string email)
        {
            var user = users.Find(user => user.Email == email);
            return user;
        }

        public User UpdateUser(User user)
        {
            User existingUser = GetUser(user.UserId);
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;
            existingUser.ConfirmPassword = user.Password;
            existingUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            return existingUser;
        }

        public bool DeleteUser(User user)
        {
            return users.Remove(user);
        }

        public User UserLogin(Login login)
        {
            var user = users.Find(x => x.Email == login.Email && x.Password == login.Password);
            return user;
        }

        public User ForgotPassword(string email)
        {
            User existingUser = users.Find(user => user.Email == email);
            if (existingUser != null)
                return existingUser;
            return null;
        }

        public User ResetPassword(User existingUser, string newPassword)
        {
            existingUser.Password = newPassword;
            existingUser.ConfirmPassword = newPassword;
            existingUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            return existingUser;
        }

        public string Authenticate(Login login)
        {
            throw new NotImplementedException();
        }
    }
}
