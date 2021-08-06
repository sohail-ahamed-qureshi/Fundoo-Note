﻿using CommonLayer;
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
        List<ResponseNotes> GetAllTrashedNotes(string userEmail);

        bool RestoreTrash(int notesId, string userEmail);
        bool ArchieveNote(int notesId, string userEmail);
        List<ResponseNotes> GetAllArchievedNotes(string userEmail);
        bool UnArchieveNote(int notesId, string userEmail);
    }
}
