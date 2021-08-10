﻿using BusinessLayer.Interfaces;
using CommonLayer;
using Fundoo.CommonLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RepositoryLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fundoo.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly IUserBL userBL;
        private readonly INotesBL notesBL;
        //distributed cache class
        private readonly IDistributedCache distributedCache;
        //cache key
        private readonly string cacheKey;
        //cache notelist
        List<ResponseNotes> notesList;
        public NotesController(INotesBL notesBL, IUserBL userBL, IDistributedCache distributedCache)
        {
            this.notesBL = notesBL;
            this.userBL = userBL;
            this.distributedCache = distributedCache;
            cacheKey = "Notes";
            notesList = new List<ResponseNotes>();
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
        /// api to retrieve all notes - implementing cache memory
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllNotesUsingRedisCache()
        {
            try
            {
                string serializedNotesList;
                var redisNotesList = await distributedCache.GetAsync(cacheKey);
                if (redisNotesList == null)
                {
                    string userEmail = GetEmailFromToken();
                    notesList = notesBL.GetAllNotes(userEmail);
                    serializedNotesList = JsonConvert.SerializeObject(notesList);
                    redisNotesList = Encoding.UTF8.GetBytes(serializedNotesList);
                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                    await distributedCache.SetAsync(cacheKey, redisNotesList, options);
                }
                else
                {
                    serializedNotesList = Encoding.UTF8.GetString(redisNotesList);
                    notesList = JsonConvert.DeserializeObject<List<ResponseNotes>>(serializedNotesList);
                }
                if (notesList.Count == 0)
                    return Ok(new { Success = true, Message = $"You have {notesList.Count} Notes." });
                return Ok(new { Success = true, Message = $"You have {notesList.Count} Notes.", data = notesList });
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
                    distributedCache.Remove(cacheKey);
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
                    List<ResponseNotes> trashedNotes = notesBL.GetAllTrashedNotes(userEmail);
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
                    distributedCache.Remove(cacheKey);
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
                    List<ResponseNotes> archievedNotes = notesBL.GetAllArchievedNotes(userEmail);
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
                    distributedCache.Remove(cacheKey);
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
                        distributedCache.Remove(cacheKey);
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
        [HttpGet("Reminders")]
        public ActionResult GetAllReminderNotes()
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail != null)
                {
                    List<ResponseNotes> reminderList = notesBL.ReminderNotes(userEmail);
                    if (reminderList.Count > 0)
                    {
                        return Ok(new { Success = true, Message = $"You have {reminderList.Count} reminders", data = reminderList });
                    }
                }
                return Ok(new { Success = true, Message = "Reminder list is Empty" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to add new label to user table
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        [HttpPost("Label")]
        public ActionResult CreateLable([FromBody] LabelRequest label)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                if (userEmail == null)
                {
                    return BadRequest(new { Message = "Invalid User Email" });
                }
                var existingUser = userBL.GetUser(userEmail);
                bool isCreated = notesBL.CreateLabel(label, existingUser);
                if (isCreated)
                {
                    return Ok(new { Success = true, Message = $"{label.LabelName} Label has been created" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
            return BadRequest();
        }
        /// <summary>
        /// ability to get all labels of user
        /// </summary>
        /// <returns></returns>
        [HttpGet("Labels")]
        public ActionResult GetAllLabel()
        {
            try
            {
                string userEmail = GetEmailFromToken();
                var labelList = notesBL.GetAllLabels(userEmail);
                if (labelList.Count > 0)
                {
                    return Ok(new { Success = true, Message = $" You have {labelList.Count} labels", Labels = labelList });
                }
                return Ok(new { Success = true, Message = $" Labels list is Empty" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// api to delete a label from database of particular user
        /// </summary>
        /// <param name="labelId"></param>
        /// <returns></returns>
        [HttpDelete("{labelId}/Label")]
        public ActionResult DeleteLabel([FromRoute]int labelId)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                User existingUser = userBL.GetUser(userEmail);
                bool result = notesBL.DeleteLabel( labelId, existingUser);
                if (result)
                {
                    return Ok(new { Success = true, Message = $" Label was Deleted"});
                }
                return NotFound(new { Success = false, Message = $"Invalid LabelId" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
        /// <summary>
        /// ability to add a label tag to a particular note of a 
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [HttpPost("TagLabel")]
        public ActionResult TagALabel(TagRequest tag)
        {
            try
            {
                string userEmail = GetEmailFromToken();
                bool result = notesBL.TagANote(tag.NoteId, tag.LabelId, userEmail);
                if (result)
                {
                    return Ok(new { Success = true, Message = $" Label was Added" });
                }
                return Ok(new { Success = true, Message = $" Invalid Data" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, ex.Message });
            }
        }
    }
}
