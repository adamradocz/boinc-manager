using Microsoft.EntityFrameworkCore;
using System.IO;

namespace BoincManager.Models
{
    // From:
    // - https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite
    // - https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/new-db?tabs=visual-studio
    // - https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/model?view=aspnetcore-3.0&tabs=visual-studio
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Host> Host { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Path.Combine(Utils.GetApplicationDataFolderPath(), Constants.DatabaseFileName)}");
        }
    }
}
