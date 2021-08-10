using CommonLayer;
using Fundoo.CommonLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRL
    {
        Note AddNewNote(Note newNote);
        List<ResponseNotes> GetAllNotes(string email);
        int IsTrash(int notesId, string userEmail);

        Note GetNoteById(int noteId);
        List<ResponseNotes> GetTrashedNotes(string userEmail);
        int ArchieveNote(int notesId, string userEmail);
        List<ResponseNotes> GetAllArchievedNotes(string userEmail);
        int PinNote(int notesId, string userEmail);
        bool DeleteNote(int notesId, string userEmail);
        UpdateNotes UpdateNote(UpdateNotes data, string userEmail);
        List<ResponseNotes> ReminderNotes(string userEmail);

        //label crud operations
        bool CreateLabel(Label label);
        List<LabelResponse> GetAllLabels(string UserEmail);
        bool DeleteLabel(int labelId, User existingUser);

        bool TagANote(int noteId, int labelId, string userEmail);
    }
}
