﻿using Fundoo.CommonLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonLayer
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoteId { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Reminder { get; set; }
        [DefaultValue(false)]
        public bool isArchieve { get; set; }
        [DefaultValue(false)]
        public bool isTrash { get; set; }
        [DefaultValue(false)]
        public bool isPin { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
        public User User { get; set; }
    }

    public class ResponseNotes 
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool isArchieve { get; set; }
        public bool isTrash { get; set; }
        public bool isPin { get; set; }
        public string Color { get; set; }
        public string Image { get; set; }
    }
}
