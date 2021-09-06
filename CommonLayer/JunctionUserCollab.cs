using Fundoo.CommonLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonLayer
{
    public class JunctionUserCollab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CollabId { get; set; }
        public Note Notes { get; set; }
        public User User { get; set; }
        public string Email { get; set; }
    }

    public class CollabRequest
    {
        public int NoteId { get; set; }
        public string Email { get; set; }
    }


   

   }
