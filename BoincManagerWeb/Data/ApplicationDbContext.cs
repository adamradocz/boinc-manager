using Microsoft.EntityFrameworkCore;

namespace BoincManagerWeb.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BoincManager.Models.Host> Host { get; set; }

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

    }
}
