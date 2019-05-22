using Microsoft.EntityFrameworkCore;

namespace BoincManagerWeb.Models
{
    // From:
    // - https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=visual-studio
    // - https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/model?view=aspnetcore-3.0&tabs=visual-studio
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BoincManager.Models.Host> Host { get; set; }

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
