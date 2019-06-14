using BoincManager;
using BoincManager.Models;
using BoincManagerMobile.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Xamarin.Forms;

namespace BoincManagerMobile.Utils
{
    public static class Storage
    {
        public static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(DependencyService.Get<IFileHelper>().GetDatabaseFolderPath(), Constants.DatabaseFileName)
            };

            var contextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            contextBuilder.UseSqlite(connectionStringBuilder.ConnectionString);
            return contextBuilder.Options;
        }
    }
}
