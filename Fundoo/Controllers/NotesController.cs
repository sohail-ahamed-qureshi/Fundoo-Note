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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            return User.FindFirst(user => user.Type == ClaimTypes.Email).Value;
        }
        /// <summary>
        /// Api to get all notes of user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAllNotes()
        {
            try
            {
                string userEmail = GetEmailFromToken();
                List<ResponseNotes> allNotes = notesBL.GetAllNotes(userEmail);
                if (allNotes.Count > 0)
                    return Ok(new { Success = true, Message = $"You have {allNotes.Count} Notes.", data = allNotes });
                return Ok(new { Success = true, Message = "You dont have any Notes." });
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
        [HttpDelete]
        [Route("{notesId}/trash")]
        public ActionResult TrashANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    int isTrashed = notesBL.IsTrash(notesId, userEmail);
                    if (isTrashed == 1)
                    {
                        return Ok(new { Success = true, Message = "Note has been Deleted!!" });
                    }
                    if (isTrashed == 0)
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
        /// api to view all the trashed notes present in trash folder.
        /// </summary>
        /// <returns></returns>
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
                        return Ok(new { Success = true, Message = $" trash has {trashedNotes.Count} Note(s)", data = trashedNotes });
                }
                return Ok(new { Success = true, Message = " trash is Empty" });
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
        [HttpPut]
        [Route("{notesId}/Archive")]
        public ActionResult ArchieveNote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    int isArchieve = notesBL.ArchieveNote(notesId, userEmail);
                    if (isArchieve == 1)
                    {
                        return Ok(new { Success = true, Message = "Note has been Archived!!" });
                    }
                    if (isArchieve == 0)
                    {
                        return Ok(new { Success = true, Message = "Note has been UnArchived!!" });
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
                        return Ok(new { Success = true, Message = $" Archive has {archievedNotes.Count} Note(s)", data = archievedNotes });
                }
                return Ok(new { Success = true, Message = " Archive is Empty" });
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
        [HttpPut]
        [Route("{notesId}/Pin")]
        public ActionResult PinANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    int isPinned = notesBL.PinNote(notesId, userEmail);
                    if (isPinned == 1)
                    {
                        return Ok(new { Success = true, Message = "Note has been Pinned!!" });
                    }
                    if (isPinned == 0)
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
        /// <summary>
        /// api to Delete a note.
        /// </summary>
        /// <param name="notesId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{notesId}")]
        public ActionResult DeleteANote([FromRoute] int notesId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    bool isDeleted = notesBL.DeleteNote(notesId, userEmail);
                    if (isDeleted)
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
        [HttpPut]
        public ActionResult UpdateANote([FromBody] UpdateNotes data)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    UpdateNotes isUpdated = notesBL.UpdateNote(data, userEmail);
                    if (isUpdated != null)
                    {
                        return Ok(new { Success = true, Message = "Note has been Updated!!", data = isUpdated });
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
