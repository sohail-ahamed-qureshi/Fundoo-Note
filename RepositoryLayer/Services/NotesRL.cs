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
        public NotesRL(UserContext context)
        {
            this.context = context;
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
        public List<ResponseNotes> GetAllNotes(string email)
        {
            List<Note> allNotes = context.DbNotes.Include(user => user.User).ToList().FindAll(note => note.Email == email && note.isArchieve == false && note.isTrash == false);
            List<ResponseNotes> responseNotesList = new List<ResponseNotes>();
            foreach (Note item in allNotes)
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
                responseNotesList.Add(responseNotes);
            }
            //getting all pinned lists.
            var pinnedList = responseNotesList.FindAll(notes => notes.isPin == true);
            List<ResponseNotes> finalResponseList = new List<ResponseNotes>();
            finalResponseList.AddRange(pinnedList);
            finalResponseList = finalResponseList.Concat(responseNotesList.Where(notes => notes.isPin == false)).ToList();
            return finalResponseList;
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
            int isTrashed = 0;
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false)
            {
                existingNote.isTrash = true;
                existingNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                isTrashed = 1;
            }
            else if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == true)
            {
                existingNote.isTrash = false;
                existingNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                isTrashed = 0;
            }
            int row = context.SaveChanges();
            return row == 1 ? isTrashed : -1;
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
            List<ResponseNotes> trashedResponseNotes = new List<ResponseNotes>();
            foreach (Note item in trashedNotes)
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
                trashedResponseNotes.Add(responseNotes);
            }
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
            int isArchieved = 0;
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isArchieve == false)
            {
                existingNote.isArchieve = true;
                existingNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                isArchieved = 1;
            }
            else if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isArchieve == true)
            {
                existingNote.isArchieve = false;
                existingNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                isArchieved = 0;
            }
            int row = context.SaveChanges();
            return row == 1 ? isArchieved : -1; 
        }
        /// <summary>
        /// ability to retreieve all active archieved notes of users and checking they are not trashed
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ResponseNotes> GetAllArchievedNotes(string userEmail)
        {
            List<Note> trashedNotes = context.DbNotes.Include(user => user.User).ToList().FindAll(note => note.Email == userEmail && note.isTrash == false && note.isArchieve == true);
            List<ResponseNotes> trashedResponseNotes = new List<ResponseNotes>();
            foreach (Note item in trashedNotes)
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
                trashedResponseNotes.Add(responseNotes);
            }
            return trashedResponseNotes;
        }
        /// <summary>
        /// ability to pin a note to top
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool PinNote(int notesId, string userEmail)
        {
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isPin == false)
            {
                existingNote.isPin = true;
            }
            int row = context.SaveChanges();
            return row == 1;
        }
        /// <summary>
        /// ability to unPin a note to top
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool UnPinNote(int notesId, string userEmail)
        {
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isPin == true)
            {
                existingNote.isPin = false;
            }
            int row = context.SaveChanges();
            return row == 1;
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
            }
            int row = context.SaveChanges();
            return row == 1 ? data : null;
        }


    }
}
