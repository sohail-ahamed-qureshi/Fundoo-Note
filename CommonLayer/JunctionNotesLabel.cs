using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonLayer
{
    public class JunctionNotesLabel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JunctionId { get; set; }
        public Note Notes { get; set; }
        public Label Labels { get; set; }
    }
    public class TagRequest
    {
        public int NoteId { get; set; }
        public int LabelId { get; set; }
    }

    public class TagResponse
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LabelId { get; set; }
        public string LabelName { get; set; }
    }
}
