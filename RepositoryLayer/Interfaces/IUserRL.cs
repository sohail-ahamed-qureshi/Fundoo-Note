using Fundoo.CommonLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRL
    {
        User RegisterNewUser(User newUser);
        List<User> GetUsers();
        User GetUser(int userid);
        User UpdateUser(User user);
        void DeleteUser(User user);
        User UserLogin(Login login);
        string ForgotPassword(string userName);
        User ResetPassword(User user, string password);


    }
}
