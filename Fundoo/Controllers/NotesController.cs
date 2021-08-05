using BusinessLayer.Interfaces;
using CommonLayer;
using Fundoo.CommonLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fundoo.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private IUserBL userBL;
        private INotesBL notesBL;
        public NotesController(INotesBL notesBL, IUserBL userBL)
        {
            this.notesBL = notesBL;
            this.userBL = userBL;
        }
        /// <summary>
        /// controller to add notes from body of s
        /// </summary>
        /// <param name="responseNotes"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [Route("AddNotes")]
        public ActionResult AddNote([FromBody] ResponseNotes responseNotes)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail == null)
                {
                    return BadRequest(new { Message = "Invalid User Email" });
                }
                User existingUser = userBL.GetUser(userEmail);
                if (responseNotes != null && existingUser != null)
                {
                    Note noteResult = notesBL.AddNotes(responseNotes, existingUser);
                    if (noteResult != null)
                    {
                        return Created(noteResult.Email, noteResult);
                    }
                }
            }
            catch
            {
                throw;
            }
            return BadRequest();
        }
        private string GetEmailFromToken()
        {
            //getting user details from token
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            string userEmail = principal.Claims.FirstOrDefault(user => user.Type == ClaimTypes.Email).Value;
            return userEmail;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("GetAllNotes")]
        public ActionResult GetAllNotes()
        {
            try
            {
                string userEmail = GetEmailFromToken();
                List<Note> allNotes = notesBL.GetAllNotes(userEmail);
                return Ok(allNotes);
            }
            catch
            {
                throw;
            }
        }
    }
}
