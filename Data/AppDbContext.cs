using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Touring.api.Models;
using Touring.api.Data;
using Touring.api.Logic;

namespace Touring.api.Data
{
	public class AppDbContext : IdentityDbContext
    {
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

         public DbSet<Leader> Leaders { get; set; }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<DocumentLeader> DocumentsLeader { get; set; }

	}

}
