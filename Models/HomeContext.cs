using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prayingtopass.Models {
        public class HomeContext : DbContext {
            public HomeContext (DbContextOptions options) : base (options) { }
            public DbSet<User> Users { get; set; }
            public DbSet<Actvty> Activities { get; set; }
            public DbSet<Participation> Participations { get; set; }
        }
    }