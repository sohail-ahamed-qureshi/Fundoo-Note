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
        User GetUser(string email);
        User UpdateUser(User user);
        bool DeleteUser(User user);
        User UserLogin(Login login);
        User ForgotPassword(string userName);
        User ResetPassword(User existingUser, string password);


    }
}
