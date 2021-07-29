using Fundoo.CommonLayer;
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
        User ResetPassword(ResetPassword resetPassword);

        string Authenticate(string UserEmail, int userID);

        bool ResetEmail(User user);


    }
}
