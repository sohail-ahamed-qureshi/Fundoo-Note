using CommonLayer;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
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
    }
}
