using CommonLayer;
using Fundoo.CommonLayer;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Services
{
    public class NotesRL : INotesRL
    {
        private UserContext context;
        private IUserRL userRL;
        public NotesRL(UserContext context, IUserRL userRL)
        {
            this.context = context;
            this.userRL = userRL;
        }
        /// <summary>
        /// ability to add new note to table.
        /// </summary>
        /// <param name="newNote"> data of note and details of user</param>
        /// <returns>Note model data added to database</returns>
        public Note AddNewNote(Note newNote)
        {
            context.DbNotes.Add(newNote);
            int row = context.SaveChanges();
            return row == 1 ? newNote : null;
        }
        /// <summary>
        /// ability to get all notes of the particular user
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public List<ResponseNotes> GetNotes(string email)
        {

            //apply joins for each note, check for labels associated with each note.
            List<Note> allNotes = context.DbNotes.Include(user => user.User).ToList().FindAll(note => note.Email == email && note.isArchieve == false && note.isTrash == false);
            var result = (from notes in context.DbNotes
                          join labelledNotes in context.JunctionNotesLabels
                          on notes.NoteId equals labelledNotes.Notes.NoteId
                          join labels in context.labelTable
                          on labelledNotes.Labels.LabelId equals labels.LabelId
                          select new
                          {
                              NoteId = notes.NoteId,
                              Title = notes.Title,
                              Description = notes.Description,
                              Email = notes.Email,
                              LabelId = labels.LabelId,
                              LabelName = labels.LabelName
                          }
                          ).ToList().FindAll(notes => notes.Email == email);
            List<ResponseNotes> responseNotesList = UtilityNotes(allNotes);
            //getting all pinned lists.
            var pinnedList = responseNotesList.FindAll(notes => notes.isPin == true);
            List<ResponseNotes> finalResponseList = new List<ResponseNotes>();
            finalResponseList.AddRange(pinnedList);
            finalResponseList = finalResponseList.Concat(responseNotesList.Where(notes => notes.isPin == false)).ToList();
            //var notesList = GetNotes(email);
            return finalResponseList;
        }


        public List<ResponseNotes> GetAllNotes(string email)
        {
            List<ResponseNotes> allNotes = context.DbNotes.Where(note => note.Email == email && note.isArchieve == false && note.isTrash == false).
                 Select(notes => new ResponseNotes
                 {
                     NoteId = notes.NoteId,
                     Title = notes.Title,
                     Description = notes.Description,
                     Color = notes.Color,
                     Image = notes.Image,
                     isArchieve = notes.isArchieve,
                     isPin = notes.isPin,
                     isTrash = notes.isTrash,
                     CreatedDate = notes.CreatedDate,
                     ModifiedDate = notes.ModifiedDate,
                     Reminder = notes.Reminder
                 }).ToList();
            foreach (var note in allNotes)
            {
                List<LabelResponse> labelList = context.JunctionNotesLabels.
                    Where(notes => notes.Notes.NoteId == note.NoteId).
                    Join(context.labelTable,
                    notesLabel => notesLabel.Labels.LabelId,
                    label => label.LabelId, (notesLabel, label) => new LabelResponse
                    {
                        LabelId = notesLabel.Labels.LabelId,
                        LabelName = label.LabelName
                    }).ToList();
                note.Labels = labelList;
            }

            if (allNotes.Count == 0)
            {
                return null;
            }
            return allNotes;

        }


        /// <summary>
        /// utility to retrieve note using note id
        /// </summary>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public Note GetNoteById(int noteId)
        {
            Note note = context.DbNotes.FirstOrDefault(note => note.NoteId == noteId);
            if (note != null)
                return note;
            return null;
        }
        /// <summary>
        /// ability to delete the a note.
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public int IsTrash(int notesId, string userEmail)
        {
            int isTrashed = 0, notFound = -1;
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false)
            {
                existingNote.isTrash = true;
                existingNote.ModifiedDate = DateTime.Now;
                isTrashed = 1;
            }
            else if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == true)
            {
                existingNote.isTrash = false;
                existingNote.ModifiedDate = DateTime.Now;
                isTrashed = 0;
            }
            int row = context.SaveChanges();
            return row == 1 ? isTrashed : notFound;
        }
        /// <summary>
        /// ability to get all trashed notes of the user
        /// check for isTrash is true 
        /// irrespective of whether it is archieved or pinned
        /// </summary>
        /// <param name="userEmail"> user email to validate</param>
        /// <returns> list of trashed notes of the user</returns>
        public List<ResponseNotes> GetTrashedNotes(string userEmail)
        {
            List<Note> trashedNotes = context.DbNotes.Include(user => user.User).ToList().FindAll(note => note.Email == userEmail && note.isTrash == true);
            List<ResponseNotes> trashedResponseNotes = UtilityNotes(trashedNotes);
            return trashedResponseNotes;
        }
        /// <summary>
        /// ability to archieve a note which is not trashed 
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public int ArchieveNote(int notesId, string userEmail)
        {
            int isArchieved = 0, notFound = -1;
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isArchieve == false)
            {
                existingNote.isArchieve = true;
                existingNote.ModifiedDate = DateTime.Now;
                isArchieved = 1;
            }
            else if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isArchieve == true)
            {
                existingNote.isArchieve = false;
                existingNote.ModifiedDate = DateTime.Now;
                isArchieved = 0;
            }
            int row = context.SaveChanges();
            return row == 1 ? isArchieved : notFound;
        }
        /// <summary>
        /// ability to retreieve all active archieved notes of users and checking they are not trashed
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ResponseNotes> GetAllArchievedNotes(string userEmail)
        {
            List<Note> archivedNotes = context.DbNotes.Include(user => user.User).ToList().FindAll(note => note.Email == userEmail && note.isTrash == false && note.isArchieve == true);
            List<ResponseNotes> archivedResponseNotes = UtilityNotes(archivedNotes);
            return archivedResponseNotes;
        }
        /// <summary>
        /// ability to pin a note to top
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public int PinNote(int notesId, string userEmail)
        {
            int isPinned = 0, notFound = -1;
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isPin == false)
            {
                existingNote.isPin = true;
                isPinned = 1;
            }
            else if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isPin == true)
            {
                existingNote.isPin = false;
                isPinned = 0;
            }
            int row = context.SaveChanges();
            return row == 1 ? isPinned : notFound;
        }
        /// <summary>
        /// ability to hard Delete a note from table
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool DeleteNote(int notesId, string userEmail)
        {
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == true)
            {
                context.DbNotes.Remove(existingNote);
            }
            int row = context.SaveChanges();
            return row == 1;
        }
        /// <summary>
        /// ability to update a note
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public UpdateNotes UpdateNote(UpdateNotes data, string userEmail)
        {
            Note existingNote = GetNoteById(data.NoteId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false)
            {
                existingNote.Title = data.Title;
                existingNote.Description = data.description;
                existingNote.Color = data.Color;
                existingNote.ModifiedDate = DateTime.Now;
            }
            int row = context.SaveChanges();
            return row == 1 ? data : null;
        }
        /// <summary>
        /// ability to display Reminders notes
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ResponseNotes> ReminderNotes(string userEmail)
        {
            List<Note> reminderNotes = context.DbNotes.Include(user => user.User).ToList().FindAll(note => note.Email == userEmail && note.isTrash == false && note.Reminder != null);
            List<ResponseNotes> responseReminderNotes = UtilityNotes(reminderNotes);
            return responseReminderNotes;
        }
        /// <summary>
        /// utility method to return response list displaying only fields that are required for user.
        /// </summary>
        /// <param name="utilityNotesList"></param>
        /// <returns></returns>
        private List<ResponseNotes> UtilityNotes(List<Note> utilityNotesList)
        {
            List<ResponseNotes> ResponseNotes = new List<ResponseNotes>();
            foreach (Note item in utilityNotesList)
            {
                ResponseNotes responseNotes = new ResponseNotes();
                responseNotes.NoteId = item.NoteId;
                responseNotes.Title = item.Title;
                responseNotes.Description = item.Description;
                responseNotes.isArchieve = item.isArchieve;
                responseNotes.isPin = item.isPin;
                responseNotes.isTrash = item.isTrash;
                responseNotes.Color = item.Color;
                responseNotes.Image = item.Image;
                responseNotes.Reminder = item.Reminder;
                responseNotes.UserId = item.User.UserId;
                responseNotes.ModifiedDate = item.ModifiedDate;
                responseNotes.CreatedDate = item.CreatedDate;
                ResponseNotes.Add(responseNotes);
            }
            return ResponseNotes;
        }

        //Ability to perform crud operations on label
        public bool CreateLabel(Label newlabel)
        {
            Label existinglabel = context.labelTable.FirstOrDefault(label => label.LabelName == newlabel.LabelName && label.Email == newlabel.Email);
            if (existinglabel == null)
            {
                context.labelTable.Add(newlabel);
            }
            int row = context.SaveChanges();
            return row == 1;
        }

        public List<LabelResponse> GetAllLabels(string userEmail)
        {
            var labels = context.labelTable.Include(user => user.User).ToList().FindAll(label => label.Email == userEmail);
            List<LabelResponse> labelLists = new List<LabelResponse>();
            foreach (var item in labels)
            {
                LabelResponse label = new LabelResponse();
                label.LabelId = item.LabelId;
                label.LabelName = item.LabelName;
                label.UserId = item.User.UserId;
                labelLists.Add(label);
            }
            return labelLists;
        }

        /// <summary>
        /// ability to update label of user
        /// </summary>
        /// <param name="data"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public LabelResponse UpdateLabel(LabelResponse data, string userEmail)
        {
            Label existingLabel = GetLabelById(data.LabelId, userEmail);
            if (existingLabel != null && existingLabel.Email == userEmail)
            {
                existingLabel.LabelName = data.LabelName;
            }
            int row = context.SaveChanges();
            return row == 1 ? data : null;
        }

        /// <summary>
        /// ability to delete a label from table
        /// </summary>
        /// <param name="labelId"></param>
        /// <param name="existingUser"></param>
        /// <returns></returns>
        public bool DeleteLabel(int labelId, User existingUser)
        {
            Label existingLabel = GetLabelById(labelId, existingUser.Email);
            if (existingLabel != null)
                context.labelTable.Remove(existingLabel);
            int row = context.SaveChanges();
            return row == 1;
        }
        /// <summary>
        /// utility method to get label from database
        /// </summary>
        /// <param name="labelId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private Label GetLabelById(int labelId, string email)
        {
            return context.labelTable.FirstOrDefault(label => label.LabelId == labelId && label.Email == email);
        }
        /// <summary>
        /// ability to add a label tag to a note
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="labelId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool TagANote(int noteId, int labelId, string userEmail)
        {
            //first check whether the note and label are already present in the junction table.
            //if not present, check whether label is already existing in labels table,
            //and also check for notes whether notes is already existing in notes table,
            //if not present add them to particular table 
            //and finally add them to junctions table.
            Note note = GetNoteById(noteId);
            Label label = GetLabelById(labelId, userEmail);
            var existingLabelledNote = context.JunctionNotesLabels.FirstOrDefault(labelledNote => labelledNote.Labels == label && labelledNote.Notes == note);
            if (existingLabelledNote == null)
            {
                //note is not labelled 
                JunctionNotesLabel junction = new JunctionNotesLabel();
                junction.Labels = label;
                junction.Notes = note;
                context.JunctionNotesLabels.Add(junction);
            }
            int row = context.SaveChanges();
            return row == 1;
        }
        /// <summary>
        /// ability to get all note that are labeled by user
        /// </summary>
        public List<TagResponse> GetAllLabeledNotes(int labelId)
        {
            var labelList = context.JunctionNotesLabels.Include(label => label.Labels).Include(notes => notes.Notes).ToList();
            var finalLabelList = labelList.FindAll(label => label.Labels.LabelId == labelId);
            List<TagResponse> tagList = new List<TagResponse>();
            foreach (var item in finalLabelList)
            {
                TagResponse tagResponse = new TagResponse();
                tagResponse.NoteId = item.Notes.NoteId;
                tagResponse.Title = item.Notes.Title;
                tagResponse.Description = item.Notes.Description;
                tagResponse.LabelId = item.Labels.LabelId;
                tagResponse.LabelName = item.Labels.LabelName;
                tagList.Add(tagResponse);
            }
            return tagList;
        }

        public bool DeletelabelfromNote(TagRequest data)
        {

            context.JunctionNotesLabels.Remove(
                context.JunctionNotesLabels.FirstOrDefault(notesLabel => notesLabel.Notes.NoteId == data.NoteId && notesLabel.Labels.LabelId == data.LabelId));
            int row = context.SaveChanges();
            return row == 1;
        }

        // Collab CRUD part


        public bool AddCollaborator(CollabRequest data, int userId)
        {
            User existingUser = userRL.GetUser(data.Email);
            Note existingNote = GetNoteById(data.NoteId);
            if (existingUser == null || existingNote == null)
            {
                return false;
            }

            var existingCollab = context.JunctionUserCollabs.FirstOrDefault(collab => collab.User.UserId == existingUser.UserId && collab.Notes.NoteId == existingNote.NoteId);
            if (existingCollab == null)
            {
                JunctionUserCollab collab = new JunctionUserCollab
                {
                    Notes = existingNote,
                    User = existingUser,
                    Email = existingUser.Email,
                    OwnerId = userId
                };
                context.JunctionUserCollabs.Add(collab);
            }
            int row = context.SaveChanges();
            return row == 1;
        }


        public List<CollabResponse> GetAllCollabs(int UserId)
        {
            List<CollabResponse> allCollabs = context.JunctionUserCollabs.Where(collab => collab.OwnerId == UserId).
                Select(collabs => new CollabResponse
                {
                    Notes = collabs.Notes,
                    UserId = collabs.User.UserId,
                    Email = collabs.Email
                }).ToList();
            return allCollabs;
        }
    }
}
