using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fundoo.CommonLayer
{
    /// <summary>
    /// model class for user registration.
    /// </summary>
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CreatedDateTime { get; set; }
        public string UpdatedDateTime { get; set; }

    }
    /// <summary>
    /// model class for login
    /// </summary>
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    /// <summary>
    /// model class to reset password
    /// </summary>
    public class ResetPassword
    {
       // public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UserResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
