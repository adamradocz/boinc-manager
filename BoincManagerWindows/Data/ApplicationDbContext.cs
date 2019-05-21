using Microsoft.EntityFrameworkCore;

namespace BoincManagerWindows.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BoincManager.Models.Host> Host { get; set; }

        /*public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }*/

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=boincmanager.db");
        }
    }
}
