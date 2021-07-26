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
        public bool DeleteUser(User user)
        {
            userContext.FundooUsers.Remove(user);
            int rowsAffected = userContext.SaveChanges();
            bool result = rowsAffected == 1 ? true : false;
            return result;
        }

        public User ForgotPassword(string email)
        {
            var user = GetUser(email);
            if (user == null)
                return null;
            return user;
        }

        public User GetUser(int userid)
        {
            var user = userContext.FundooUsers.Find(userid);
            return user;
        }

        public User GetUser(string email)
        {
            var user = userContext.FundooUsers.FirstOrDefault(user => user.Email == email);
            return user;
        }

        public List<User> GetUsers()
        {
            return userContext.FundooUsers.ToList();
        }

        public User RegisterNewUser(User newUser)
        {
            var existingUser = GetUser(newUser.Email);
            if(existingUser == null){
                newUser.CreatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                newUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                userContext.FundooUsers.Add(newUser);
            }
            int rowsAffected = userContext.SaveChanges();
            return rowsAffected == 1 ? newUser : null;
        }

        public User ResetPassword(User existingUser, string newPassword)
        {
            existingUser.Password = newPassword;
            existingUser.ConfirmPassword = newPassword;
            existingUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            userContext.SaveChanges();
            return existingUser;
        }

        public User UpdateUser(User user)
        {
            var existingUser = GetUser(user.Email);
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            userContext.SaveChanges();
            return existingUser;
        }

        public User UserLogin(Login login)
        {
            var user = userContext.FundooUsers.FirstOrDefault(user => user.Email == login.Email);
            return user;
        }
    }
}
