﻿using BusinessLayer.Interfaces;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Experimental.System.Messaging;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Cors;

namespace Fundoo.Controllers
{
    [EnableCors()]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserBL userBL;
        private IDistributedCache distributedCache;
        public UserController(IUserBL userBL, IDistributedCache distributedCache)
        {
            this.userBL = userBL;
            this.distributedCache = distributedCache;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public ActionResult RegisterNewUser(User newUser)
        {
            User user = userBL.UserRegister(newUser);
            if (user != null)
            {
                UserResponse userResponse = new UserResponse();
                userResponse.FirstName = newUser.FirstName;
                userResponse.LastName = newUser.LastName;
                userResponse.Email = newUser.Email;
                return Created(userResponse.Email, userResponse);
            }
            return BadRequest("User Already Exists!!");
        }
        //public ActionResult GetAllUsers()
        //{
        //    var users = userBL.GetUsers();
        //    return Ok(users);
        //}

        //public ActionResult GetUser(int userid)
        //{
        //    var user = userBL.GetUser(userid);
        //    if (user != null)
        //        return Ok(user);
        //    return NotFound($"UserID: {userid} Not Found!!");
        //}
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public ActionResult Login(Login login)
        {
            var user = userBL.UserLogin(login);
            if (user != null)
            {
                distributedCache.Remove("Notes");
                string token = userBL.Authenticate(user.Email, user.UserId);
                return Ok(new { Success = true, Message = $"Login Successfull, Welcome {user.FirstName + " " + user.LastName}", data = token });
            }
            return NotFound("Invalid UserName or Password");
        }

        //public ActionResult UpdateUserDetails(User user)
        //{
        //    User updatedUser = null;
        //    if (user != null)
        //        updatedUser = userBL.UpdateUser(user);
        //    if (updatedUser != null)
        //        return Created(updatedUser.Email, updatedUser);
        //    return NotFound($"Invalid User Details");
        //}

        //[HttpDelete]
        //[Route("{userId}")]
        //public ActionResult DeleteUser(int userId)
        //{
        //    bool result = userBL.DeleteUser(userId);
        //    if (result)
        //        return Ok(new { Success = true, Message = $"User Id: {userId} Delete SuccessFull " });
        //    return NotFound($"Invalid Id: {userId}, User Not Found");
        //}
        [AllowAnonymous]
        [HttpPost]
        [Route("forgotpassword")]
        public ActionResult Forgotpassword(User user)
        {
            var existingUser = userBL.ForgotPassword(user.Email);
            if (existingUser != null)
            {
                //send email to user for reset password
                userBL.SendMessageQueue(existingUser);
                Task.Delay(5000);
                return Ok(new { Success = true, Message = $"Password Reset Link has been sent to Registered Email: {existingUser.Email}" });
            }
            return NotFound("Invalid Email");
        }
       
        [HttpPut]
        [Route("resetpassword/{token}")]
        public ActionResult ResetPassword([FromRoute] string token, [FromBody] ResetPassword reset)
        {
            if (reset != null && token != null)
            {
                //extracting userId from token
                ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
                int userId = Convert.ToInt32(principal.Claims.SingleOrDefault(c => c.Type == "userId").Value);
                if (userId != 0)
                {
                    User updatedUser = userBL.ResetPassword(userId, reset);
                    if (updatedUser.UserId != 0)
                    {
                        return Ok(new { Success = true, Message = $"Reset Password Successfully at {updatedUser.UpdatedDateTime}", Data = updatedUser });
                    }
                }
            }
            return NotFound($"Invalid User Details");
        }
    }
}
