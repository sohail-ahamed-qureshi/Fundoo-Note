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

        public List<ResponseNotes> GetAllNotes(string email)
        {
            List<Note> allNotes = context.DbNotes.ToList().FindAll(note => note.Email == email && note.isArchieve == false && note.isTrash == false );
            ResponseNotes responseNotes = new ResponseNotes();
            List<ResponseNotes> responseNotesList = new List<ResponseNotes>();
            foreach (Note item in allNotes)
            {
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
    }
}
