using BusinessLayer.Interfaces;
using CommonLayer;
using Fundoo.CommonLayer;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Services
{
    public class NotesBL : INotesBL
    {
        private INotesRL notesRL;
        public NotesBL(INotesRL notesRL)
        {
            this.notesRL = notesRL;
        }
        /// <summary>
        /// update Notes model using response model with existing user
        /// </summary>
        /// <param name="responseNotes"></param>
        /// <param name="existingUser"></param>
        /// <returns></returns>
        public Note AddNotes(ResponseNotes responseNotes, User existingUser)
        {
            if (responseNotes != null && existingUser != null)
            {
                //setting up New Note
                Note newNote = new Note();
                newNote.Email = existingUser.Email;
                newNote.Title = responseNotes.Title;
                newNote.Description = responseNotes.Description;
                //newNote.Reminder = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                newNote.Reminder = DateTime.Now;
                newNote.CreatedDate = DateTime.Now;
                newNote.ModifiedDate =DateTime.Now;
                newNote.Color = responseNotes.Color;
                newNote.Image = responseNotes.Image;
                newNote.isArchieve = responseNotes.isArchieve;
                newNote.isPin = responseNotes.isPin;
                newNote.User = existingUser;
                Note noteResult = notesRL.AddNewNote(newNote);
                if (noteResult != null)
                    return noteResult;
                return null;
            }
            return null;
        }
        /// <summary>
        /// validate user email and get all notes of particalar user
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ResponseNotes> GetAllNotes(string userEmail)
        {
            if (userEmail != null)
            {
                List<ResponseNotes> allNotes = notesRL.GetAllNotes(userEmail);
                return allNotes;
            }
            return null;
        }
        /// <summary>
        /// utility method to get note by noteid
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public Note GetNoteById(int noteId)
        {
            if (noteId > 0)
            {
                Note note = notesRL.GetNoteById(noteId);
                if (note != null)
                    return note;
            }
            return null;
        }
        /// <summary>
        /// ability to validate notes id and user email before trashing a note.
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public int IsTrash(int notesId, string userEmail)
        {
            if (notesId > 0 && userEmail != null)
                return notesRL.IsTrash(notesId, userEmail);
            return -1;
        }
        /// <summary>
        /// ability to validate userEmail before getting all the trashed notes
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ResponseNotes> GetAllTrashedNotes(string userEmail)
        {
            try
            {
                if (userEmail != null)
                {
                    var trashedNotes = notesRL.GetTrashedNotes(userEmail);
                    return trashedNotes;
                }
            }
            catch
            {
                throw;
            }
            return null;
        }
        /// <summary>
        /// validating notes Id and userEmail before Archieving a note
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public int ArchieveNote(int notesId, string userEmail)
        {
            if (notesId > 0 && userEmail != null)
                return notesRL.ArchieveNote(notesId, userEmail);
            return -1;
        }
        /// <summary>
        /// ability to validate userEmail before getting all the archieved notes
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ResponseNotes> GetAllArchievedNotes(string userEmail)
        {
            try
            {
                if (userEmail != null)
                {
                    var archeievedNotes = notesRL.GetAllArchievedNotes(userEmail);
                    return archeievedNotes;
                }
            }
            catch
            {
                throw;
            }
            return null;
        }
        /// <summary>
        /// validation for pin note
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public int PinNote(int notesId, string userEmail)
        {
            if (notesId > 0 && userEmail != null)
                return notesRL.PinNote(notesId, userEmail);
            return -1;
        }
        /// <summary>
        /// ability to permanently delete a note from database
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool DeleteNote(int notesId, string userEmail)
        {
            if (notesId > 0 && userEmail != null)
                return notesRL.DeleteNote(notesId, userEmail);
            return false;
        }
        /// <summary>
        /// ability to update a note
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public UpdateNotes UpdateNote(UpdateNotes data, string userEmail)
        {
            if (data != null && userEmail != null)
            {
                return notesRL.UpdateNote(data, userEmail);
            }
            return null;
        }

        public List<ResponseNotes> ReminderNotes(string userEmail)
        {
            if (userEmail != null)
            {
                var reminderNotes = notesRL.ReminderNotes(userEmail);
                return reminderNotes;
            }
            return null;
        }

        //label crud operations
        public bool CreateLabel(LabelRequest label, User existingUser)
        {
            if (label != null && existingUser != null)
            {
                Label newLabel = new Label();
                newLabel.LabelName = label.LabelName;
                newLabel.User = existingUser;
                newLabel.Email = existingUser.Email;
                return notesRL.CreateLabel(newLabel);
            }

            return false;
        }
        /// <summary>
        /// ability to get all labels from table
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<LabelResponse> GetAllLabels(string userEmail)
        {
            return notesRL.GetAllLabels(userEmail);
        }
        /// <summary>
        /// ability to delete a label
        /// </summary>
        /// <param name="labelId"></param>
        /// <param name="existingUser"></param>
        /// <returns></returns>
        public bool DeleteLabel(int labelId, User existingUser)
        {
            if (labelId > 0 && existingUser != null)
            {
                return notesRL.DeleteLabel(labelId, existingUser);
            }
            return false;
        }

        public LabelResponse UpdateLabel(LabelResponse data, string userEmail)
        {
            if (data != null && userEmail != null)
            {
                return notesRL.UpdateLabel(data, userEmail);
            }
            return null;
        }

        public bool TagANote(int noteId, int labelId, string userEmail)
        {
            if (noteId > 0 && labelId > 0 && userEmail != null)
            {
                return notesRL.TagANote(noteId, labelId, userEmail);
            }
            return false;
        }

        public List<TagResponse> GetAllLabeledNotes(int labelId)
        {
            if (labelId > 0)
            {
               return notesRL.GetAllLabeledNotes(labelId);
            }
            return null;
        }

        public bool DeletelabelfromNote(TagRequest data)
        {
            if(data != null)
            {
              return notesRL.DeletelabelfromNote(data);
            }
            return false;
        }


        //Collab Functionality
       
        public bool AddCollab(CollabRequest data, int userId)
        {
            if(data != null)
            {
                return notesRL.AddCollaborator(data, userId);
            }
            return false;
        }

        public List<CollabResponse> GetAllCollabs(int UserId)
        {
            return notesRL.GetAllCollabs(UserId);
        }

        public bool RemoveCollab(CollabRequest data, int userId)
        {
            if (data != null)
            {
                return notesRL.RemoveCollab(data, userId);
            }
            return false;
        }

    }
}
