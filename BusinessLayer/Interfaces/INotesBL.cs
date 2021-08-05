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
        List<OutputNotes> GetAllNotes(string userEmail);
        bool IsTrash(int notesId, string userEmail);

        Note GetNoteById(int noteId);
    }
}
