using Elearning__portal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Elearning__portal.Data
{
    public class DtabaseSet : IdentityDbContext<ApplicationUser>
    {
        
        public DtabaseSet(DbContextOptions<DtabaseSet> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Lecturer> Lecturer { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Student> Students { get; set; }

        public new DbSet<IdentityRole> Roles { get; set; }

        public DbSet<Notes> Notes { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        
    }
}
