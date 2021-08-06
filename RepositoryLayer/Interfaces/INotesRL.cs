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
        bool RestoreNote(int notesId, string userEmail);
        bool ArchieveNote(int notesId, string userEmail);
        List<ResponseNotes> GetAllArchievedNotes(string userEmail);
        bool UnArchieveNote(int notesId, string userEmail);
    }
}
