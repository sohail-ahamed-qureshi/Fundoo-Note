﻿using CommonLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRL
    {
        Note AddNewNote(Note newNote);
        List<ResponseNotes> GetAllNotes(string email);
    }
}
