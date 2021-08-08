using CommonLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRL
    {
        Note AddNewNote(Note newNote);
        List<ResponseNotes> GetAllNotes(string email);
        bool IsTrash(int notesId, string userEmail);

        Note GetNoteById(int noteId);
        List<ResponseNotes> GetTrashedNotes(string userEmail);
        bool RestoreNote(int notesId, string userEmail);
        bool ArchieveNote(int notesId, string userEmail);
        List<ResponseNotes> GetAllArchievedNotes(string userEmail);
        bool UnArchieveNote(int notesId, string userEmail);
        bool PinNote(int notesId, string userEmail);
        bool UnPinNote(int notesId, string userEmail);
        bool DeleteNote(int notesId, string userEmail);
        UpdateNotes UpdateNote(UpdateNotes data, string userEmail);
    }
}
