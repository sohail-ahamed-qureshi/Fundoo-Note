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
        List<ResponseNotes> GetAllNotes(string userEmail, int userId);
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
        LabelResponse UpdateLabel(LabelResponse data, string userEmail);
        bool DeleteLabel(int labelId, User existingUser);
        bool TagANote(int noteId, int labelId, string userEmail);

        List<TagResponse> GetAllLabeledNotes(int labelId);
         bool DeletelabelfromNote(TagRequest data);

        //Collab Crud
        bool AddCollab(CollabRequest data, int UserId);
        List<CollabResponse> GetAllCollabs(int UserId);

        bool RemoveCollab(CollabRequest data, int userId);
    }
}
