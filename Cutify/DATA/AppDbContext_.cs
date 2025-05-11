using System;
using Cutify.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cutify.DATA
{
    public class AppDbContext_ : IdentityDbContext
    {
        public AppDbContext_(DbContextOptions<AppDbContext_> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<ExceptionLog> ExceptionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);         
        }

    }
}

