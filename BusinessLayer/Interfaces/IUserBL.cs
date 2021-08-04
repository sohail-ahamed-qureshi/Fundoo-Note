using Fundoo.CommonLayer;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface IUserBL
    {
        User UserRegister(User newUser);
        List<User> GetUsers();
        User GetUser(int userid);
        User UpdateUser(User user);
        bool DeleteUser(int id);

        User UserLogin(Login login);
        User ForgotPassword(string userName);
        User ResetPassword(int userId, ResetPassword resetPassword);

        string Authenticate(string UserEmail, int userID);

        string ResetEmail(User user);

        void SendMessageQueue(User user);
    }
}
