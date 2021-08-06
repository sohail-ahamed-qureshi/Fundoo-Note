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
        /// <summary>
        /// utiltity to extract userEmail from the token.
        /// </summary>
        /// <returns></returns>
        private string GetEmailFromToken()
        {
            //getting user details from token
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            string userEmail = principal.Claims.FirstOrDefault(user => user.Type == ClaimTypes.Email).Value;
            return userEmail;
        }
        /// <summary>
        /// Api to get all notes of user
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ActionResult GetAllNotes()
        {
            try
            {
                string userEmail = GetEmailFromToken();
                List<OutputNotes> allNotes = notesBL.GetAllNotes(userEmail);
                if (allNotes.Count > 0)
                    return Ok(allNotes);
                return Ok(new { Message = "You dont have any Notes." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to delete a note
        /// </summary>
        /// <param name="notesId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{notesId}/trash")]
        public ActionResult TrashANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    bool isTrashed = notesBL.IsTrash(notesId, userEmail);
                    if (isTrashed)
                    {
                        return Ok(new { Success = true, Message = "Note has been Deleted!!" });
                    }
                }
                return NotFound(new { Success = false, Message = "Note Not Found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to view all the trashed notes present in trash folder.
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("trash")]
        public ActionResult GetAllTrashedNotes()
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    var trashedNotes = notesBL.GetAllTrashedNotes(userEmail);
                    if (trashedNotes.Count > 0)
                        return Ok(trashedNotes);
                }
                return Ok(new { Message = "Your trash is Empty" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to restore a note which is trashed
        /// </summary>
        /// <param name="notesId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{notesId}/Restore")]
        public ActionResult RestoreANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    bool isRestored = notesBL.RestoreTrash(notesId, userEmail);
                    if (isRestored)
                    {
                        return Ok(new { Success = true, Message = "Note has been Restored!!" });
                    }
                }
                return NotFound(new { Success = false, Message = "Note Not Found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to archieve a existing note of the user
        /// </summary>
        /// <param name="notesId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{notesId}/Archive")]
        public ActionResult ArchieveNote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    bool isArchieve = notesBL.ArchieveNote(notesId, userEmail);
                    if (isArchieve)
                    {
                        return Ok(new { Success = true, Message = "Note has been Archieved!!" });
                    }
                }
                return NotFound(new { Success = false, Message = "Note Not Found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to view all notes present in archieve folder
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("Archive")]
        public ActionResult GetAllArchievedNotes()
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    var archievedNotes = notesBL.GetAllArchievedNotes(userEmail);
                    if (archievedNotes.Count > 0)
                        return Ok(archievedNotes);
                }
                return Ok(new { Message = "Your Archieve is Empty" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to unarchieve already archieved note of user
        /// </summary>
        /// <param name="notesId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{notesId}/UnArchive")]
        public ActionResult UnArchieveANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    bool isRestored = notesBL.UnArchieveNote(notesId, userEmail);
                    if (isRestored)
                    {
                        return Ok(new { Success = true, Message = "Note has been UnArchieved!!" });
                    }
                }
                return NotFound(new { Success = false, Message = "Note Not Found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to pin a note to top
        /// </summary>
        /// <param name="notesId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{notesId}/Pin")]
        public ActionResult PinANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    bool isPinned = notesBL.PinNote(notesId, userEmail);
                    if (isPinned)
                    {
                        return Ok(new { Success = true, Message = "Note has been Pinned!!" });
                    }
                }
                return NotFound(new { Success = false, Message = "Note Not Found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to pin a note to top
        /// </summary>
        /// <param name="notesId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{notesId}/UnPin")]
        public ActionResult UnPinANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    bool isPinned = notesBL.UnPinNote(notesId, userEmail);
                    if (isPinned)
                    {
                        return Ok(new { Success = true, Message = "Note has been UnPinned!!" });
                    }
                }
                return NotFound(new { Success = false, Message = "Note Not Found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }


    }
}
