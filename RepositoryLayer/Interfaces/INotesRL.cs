using CommonLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRL
    {
        Note AddNewNote(Note newNote);
        List<OutputNotes> GetAllNotes(string email);
        bool IsTrash(int notesId, string userEmail);

        Note GetNoteById(int noteId);
        List<ResponseNotes> GetTrashedNotes(string userEmail);
    }
}
