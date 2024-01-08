using Microsoft.EntityFrameworkCore;
using Touring.api.Models;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
	{
	}

	public DbSet<Student> Students { get; set; }


}
