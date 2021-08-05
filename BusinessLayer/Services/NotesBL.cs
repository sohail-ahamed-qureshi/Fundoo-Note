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

        public Note AddNotes(ResponseNotes responseNotes, User existingUser)
        {
            if(responseNotes != null && existingUser != null)
            {
                //setting up New Note
                Note newNote = new Note();
                newNote.Email = existingUser.Email;
                newNote.Title = responseNotes.Title;
                newNote.Description = responseNotes.Description;
                newNote.Reminder = responseNotes.Reminder;
                newNote.CreatedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                newNote.ModifiedDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                newNote.Reminder = "no";
                newNote.Color = "white";
                newNote.Image = "NA";
                newNote.User = existingUser;
                Note noteResult =  notesRL.AddNewNote(newNote);
                return noteResult;
            }
            return null;
            

        }

        public List<Note> GetAllNotes(string userEmail)
        {
            List<Note> allNotes = notesRL.GetAllNotes(userEmail);
            return allNotes;
        }
    }
}
