using CommonLayer;
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
        public List<OutputNotes> GetAllNotes(string email)
        {
            List<Note> allNotes = context.DbNotes.ToList().FindAll(note => note.Email == email && note.isArchieve == false && note.isTrash == false);
            List<OutputNotes> responseNotesList = new List<OutputNotes>();
            foreach (Note item in allNotes)
            {
                OutputNotes responseNotes = new OutputNotes();
                responseNotes.NoteId = item.NoteId;
                responseNotes.Title = item.Title;
                responseNotes.Description = item.Description;
                responseNotes.isArchieve = item.isArchieve;
                responseNotes.isPin = item.isPin;
                responseNotes.isTrash = item.isTrash;
                responseNotes.Color = item.Color;
                responseNotes.Image = item.Image;
                responseNotes.Reminder = item.Reminder;
                responseNotesList.Add(responseNotes);
            }
            return responseNotesList;
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
        public bool IsTrash(int notesId, string userEmail)
        {
            Note existingNote = GetNoteById(notesId);
            if(existingNote!=null && existingNote.Email == userEmail)
            {
                existingNote.isTrash = true;
                existingNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
            int row = context.SaveChanges();
            return row == 1;
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
            List<Note> trashedNotes = context.DbNotes.ToList().FindAll(note =>note.Email == userEmail && note.isTrash == true);
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
                trashedResponseNotes.Add(responseNotes);
            }
            return trashedResponseNotes;
        }
        /// <summary>
        /// ability to restore a trashed note which is existing in trashed folder
        /// and rewriting isTrash to false so that is removed from trashed folder and restored back.
        /// </summary>
        /// <param name="notesId">id of note which is trashed</param>
        /// <param name="userEmail">userEmail to validate</param>
        /// <returns>boolean value</returns>
        public bool RestoreNote(int notesId, string userEmail)
        {
            Note existingNote = GetNoteById(notesId);
            if (existingNote != null && existingNote.Email == userEmail && existingNote.isTrash ==  true)
            {
                existingNote.isTrash = false;
                existingNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
            int row = context.SaveChanges();
            return row == 1;
        }
        /// <summary>
        /// ability to archieve a note which is not trashed 
        /// </summary>
        /// <param name="notesId"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool ArchieveNote(int notesId, string userEmail)
        {
            Note existingNote = GetNoteById(notesId);
            if(existingNote!= null && existingNote.Email == userEmail && existingNote.isTrash == false && existingNote.isArchieve == false)
            {
                existingNote.isArchieve = true;
                existingNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
            int row = context.SaveChanges();
            return row == 1;
        }
        /// <summary>
        /// ability to retreieve all active archieved notes of users and checking they are not trashed
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public List<ResponseNotes> GetAllArchievedNotes(string userEmail)
        {
            List<Note> trashedNotes = context.DbNotes.ToList().FindAll(note => note.Email == userEmail && note.isTrash == false && note.isArchieve == true);
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
                trashedResponseNotes.Add(responseNotes);
            }
            return trashedResponseNotes;
        }


    }
}
