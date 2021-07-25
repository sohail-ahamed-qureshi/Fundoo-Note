using BusinessLayer.Interfaces;
using Fundoo.CommonLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fundoo.Controllers
{

    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserBL userBL;
        public UserController(IUserBL userBL)
        {
            this.userBL = userBL;
        }

        [HttpPost] // create 
        [Route("api/[controller]")]
        public ActionResult RegisterNewUser(User newUser)
        {
            User user = userBL.UserRegister(newUser);
            return Created(newUser.UserId.ToString(), user);
        }

        [HttpGet] // read
        [Route("api/[controller]")]
        public ActionResult GetAllUsers()
        {
            var users = userBL.GetUsers();
            return Ok(users);
        }

        [HttpGet] //read
        [Route("api/[controller]/{userId}")]
        public ActionResult GetUser(int userid)
        {
            var user = userBL.GetUser(userid);
            if (user != null)
                return Ok(user);
            return NotFound($"UserID: {userid} Not Found!!");
        }

        [HttpPost] // create 
        [Route("api/[controller]/login")]
        public ActionResult Login(Login login)
        {
            var user = userBL.UserLogin(login);
            if (user != null)
            {
                return Ok(new { Success = true, Message = $"Login Successfull, Welcome {user.UserName}" });
            }
            return NotFound("Invalid UserName or Password");
        }

        [HttpPut] //update
        [Route("api/[controller]/Update/{userId}")]
        public ActionResult UpdateUserDetails(User user)
        {
            var updatedUser = userBL.UpdateUser(user);
            if (updatedUser != null)
            {
                return Created(updatedUser.UserId.ToString(), updatedUser);
            }
            return NotFound($"Invalid UserID: {user.UserId}");
        }

        [HttpDelete] //delete
        [Route("api/[controller]/Delete/{userId}")]
        public ActionResult DeleteUser(int userId)
        {
            bool result = userBL.DeleteUser(userId);
            if(result)
                return Ok(new { Success = true, Message = $"User id: {userId} Delete SuccessFull " });
            return NotFound($"Invalid userId: {userId}, User Not Found");
        }

        [HttpPost] //create
        [Route("api/[controller]/forgotpassword")]
        public ActionResult Forgotpassword(User user)
        {
           string password = userBL.ForgotPassword(user.UserName);
            if(password != null) 
                return Ok(new { Success = true, Message =  $"User Name: {user.UserName}, Password = {password}"  });
            return NotFound("Invalid UserName");
        }

        [HttpPut] //update
        [Route("api/[controller]/resetpassword/{userId}")]
        public ActionResult ResetPassword(int userId,User user)
        {
            User upadatedUser = userBL.ResetPassword(userId, user.password);
            if (upadatedUser != null)
            {
                return Ok(new { Success = true, Message = $"Reset Password Successfully", Data = upadatedUser });
            }
            return NotFound($"Invalid UserId: {userId}");
        }
    }
}
