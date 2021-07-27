using BusinessLayer.Interfaces;
using Fundoo.CommonLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using BusinessLayer.Services;

namespace Fundoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserBL userBL;
        public UserController(IUserBL userBL)
        {
            this.userBL = userBL;
        }

        [HttpPost]
        [Route("Register")]
        public ActionResult RegisterNewUser(User newUser)
        {
            User user = userBL.UserRegister(newUser);
            if (user != null)
                return Created(newUser.Email, user);
            return BadRequest("User Already Exists!!");
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            var users = userBL.GetUsers();
            return Ok(users);
        }

        [HttpGet]
        [Route("{userId}")]
        public ActionResult GetUser(int userid)
        {
            var user = userBL.GetUser(userid);
            if (user != null)
                return Ok(user);
            return NotFound($"UserID: {userid} Not Found!!");
        }

        [HttpPost]
        [Route("Login")]
        public ActionResult Login(Login login)
        {
            var user = userBL.UserLogin(login);
            if (user != null)
            {
                string token = userBL.Authenticate(user.Email);
                return Ok(new { Success = true, Message = $"Login Successfull, Welcome {user.FirstName + " " + user.LastName}", data = token });
            }
            return NotFound("Invalid UserName or Password");
        }

        [HttpPut]
        [Route("Update")]
        public ActionResult UpdateUserDetails(User user)
        {
            User updatedUser = null;
            if (user != null)
                updatedUser = userBL.UpdateUser(user);
            if (updatedUser != null)
                return Created(updatedUser.Email, updatedUser);
            return NotFound($"Invalid User Details");
        }

        [HttpDelete]
        [Route("{userId}")]
        public ActionResult DeleteUser(int userId)
        {
            bool result = userBL.DeleteUser(userId);
            if (result)
                return Ok(new { Success = true, Message = $"User Id: {userId} Delete SuccessFull " });
            return NotFound($"Invalid Id: {userId}, User Not Found");
        }

        [HttpPost]
        [Route("forgotpassword")]
        public ActionResult Forgotpassword(User user)
        {
            string password = userBL.ForgotPassword(user.Email);
            if (password != null)
                return Ok(new { Success = true, Message = $"Email: {user.Email}, Password = {password}" });
            return NotFound("Invalid Email");
        }

        [HttpPut]
        [Route("resetpassword")]
        public ActionResult ResetPassword(ResetPassword reset)
        {
            User upadatedUser = userBL.ResetPassword(reset);
            if (upadatedUser != null)
            {
                return Ok(new { Success = true, Message = $"Reset Password Successfully at {upadatedUser.UpdatedDateTime}", Data = upadatedUser });
            }
            return NotFound($"Invalid User Details");
        }
    }
}
