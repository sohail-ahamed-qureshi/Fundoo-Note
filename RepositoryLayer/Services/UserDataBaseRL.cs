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
        /// <summary>
        /// ability to remove user details from the database
        /// </summary>
        /// <param name="user"> existing user present in database</param>
        /// <returns></returns>
        public bool DeleteUser(User user)
        {
            userContext.FundooUsers.Remove(user);
            int rowsAffected = userContext.SaveChanges();
            bool result = rowsAffected == 1 ? true : false;
            return result;
        }
        /// <summary>
        /// ability to retrieve password from the database 
        /// after validating email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User ForgotPassword(string email)
        {
            var user = GetUser(email);
            if (user == null)
                return null;
            return user;
        }
        /// <summary>
        /// get particular user details depending upon the user id
        /// </summary>
        /// <param name="userid">unique id of user</param>
        /// <returns></returns>
        public User GetUser(int userid)
        {
            var user = userContext.FundooUsers.Find(userid);
            return user;
        }
        /// <summary>
        /// ability to retireve user details by email
        /// </summary>
        /// <param name="email">unique email address of the user</param>
        /// <returns></returns>
        public User GetUser(string email)
        {
            var user = userContext.FundooUsers.FirstOrDefault(user => user.Email == email);
            return user;
        }
        /// <summary>
        /// ability to retireve all the users present in database
        /// </summary>
        /// <returns>list of user details</returns>
        public List<User> GetUsers()
        {
            return userContext.FundooUsers.ToList();
        }
        /// <summary>
        /// register a new user
        /// add new user to database
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public User RegisterNewUser(User newUser)
        {
            var existingUser = GetUser(newUser.Email);
            if(existingUser == null){
                newUser.CreatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                newUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                newUser.ConfirmPassword = null;
                userContext.FundooUsers.Add(newUser);
            }
            int rowsAffected = userContext.SaveChanges();
            return rowsAffected == 1 ? newUser : null;
        }

        public User ResetPassword(User existingUser, string newPassword)
        {
            existingUser.Password = newPassword;
            existingUser.ConfirmPassword = null;
            existingUser.UpdatedDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            userContext.SaveChanges();
            return existingUser;
        }

        public User UpdateUser(User user)
        {
            var existingUser = GetUser(user.Email);
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Password = user.Password;
            existingUser.ConfirmPassword = null;
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
