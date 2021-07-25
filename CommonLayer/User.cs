using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fundoo.CommonLayer
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string password { get; set; }
        public int age { get; set; }
        public string Occupation { get; set; }
    }

    public class Login
    {
        public string userName { get; set; }
        public string password { get; set; }
    }
}
