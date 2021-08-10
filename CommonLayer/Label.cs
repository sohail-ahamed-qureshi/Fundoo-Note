using Fundoo.CommonLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CommonLayer
{
    public class Label
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabelId { get; set; }
        [Required]
        public string LabelName { get; set; }
        public string Email { get; set; }
        public User User { get; set; }
    }

    public class LabelResponse
    {
        public int LabelId { get; set; }
        public string LabelName { get; set; }
        public int UserId { get; set; }
    }

    public class LabelRequest
    {
        public string LabelName { get; set; }
    }
}
