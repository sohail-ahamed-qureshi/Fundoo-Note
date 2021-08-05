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
        List<Note> GetAllNotes(string userEmail);
    }
}
