using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommonLayer
{
    public class UpdateNotes
    {
        public int NoteId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string description { get; set; }
        public string Color { get; set; }
    }
}
