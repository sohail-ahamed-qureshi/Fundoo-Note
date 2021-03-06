using CommonLayer;
using Fundoo.CommonLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Services
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        public DbSet<User> FundooUsers { get; set; }
        public DbSet<Note> DbNotes { get; set; }
        public DbSet<Label> labelTable { get; set; }
        public DbSet<JunctionNotesLabel> JunctionNotesLabels { get; set; }
        public DbSet<JunctionUserCollab> JunctionUserCollabs { get; set; }
    }
}
