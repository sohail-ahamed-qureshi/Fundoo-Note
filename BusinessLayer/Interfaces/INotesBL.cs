using CommonLayer;
using Fundoo.CommonLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interfaces
{
    public interface INotesBL
    {
        Note AddNotes(ResponseNotes responseNotes, User user);
        List<ResponseNotes> GetAllNotes(string userEmail);
        int IsTrash(int notesId, string userEmail);

        Note GetNoteById(int noteId);
        List<ResponseNotes> GetAllTrashedNotes(string userEmail);
        int ArchieveNote(int notesId, string userEmail);
        List<ResponseNotes> GetAllArchievedNotes(string userEmail);
        int PinNote(int notesId, string userEmail);
        bool DeleteNote(int notesId, string userEmail);
        UpdateNotes UpdateNote(UpdateNotes data, string userEmail);

        List<ResponseNotes> ReminderNotes(string userEmail);


        bool CreateLabel(LabelRequest label, User existingUser);
        List<LabelResponse> GetAllLabels(string UserEmail);
        bool DeleteLabel(int labelId, User existingUser);
        bool TagANote(int noteId, int labelId, string userEmail);

        List<TagResponse> GetAllLabeledNotes(int labelId);
    }
}
