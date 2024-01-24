using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Touring.api.Data; // Add the correct namespace for AppDbContext

namespace Touring.api.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                // Ensure you have the correct namespace for AppDbContext
                using (AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    dbContext.Database.Migrate();
                }
            }
        }
    }
}
