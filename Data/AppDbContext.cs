using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Touring.api.Models;

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

        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Student> Students { get; set; }


	}

}
