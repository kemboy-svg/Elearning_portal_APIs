using Elearning__portal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Elearning__portal.Data
{
    public class DtabaseSet : IdentityDbContext<ApplicationUser>
    {
        
        public DtabaseSet(DbContextOptions<DtabaseSet> options) : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //         builder.Entity<Lecturer>()
        //        .HasOne(l => l.AssignedUnit)
        //        .WithMany()
        //        .HasForeignKey(l => l.UnitId)
        //        .OnDelete(DeleteBehavior.SetNull);

            // builder.Entity<Student>()
            //.HasOne(s => s.Unit)
            //.WithMany(u => u.Students)
            //.HasForeignKey(s => s.UnitId)
            //.OnDelete(DeleteBehavior.Restrict);

            // builder.Entity<Notes>()
            // .HasOne(n => n.Unit)
            // .WithMany(u => u.Notes)
            // .HasForeignKey(a => a.UnitId)
            // .OnDelete(DeleteBehavior.Restrict);

            // builder.Entity<Assignment>()
            // .HasOne(a => a.Unit)
            // .WithMany(u => u.Assignments)
            // .HasForeignKey(a =>a.UnitId)
            // .OnDelete(DeleteBehavior.Restrict);
       // }

        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Student> Students { get; set; }

        public new DbSet<IdentityRole> Roles { get; set; }

        public DbSet<Notes> Notes { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        
    }
}
