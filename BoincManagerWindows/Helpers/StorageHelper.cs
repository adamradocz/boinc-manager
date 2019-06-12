using BoincManager;
using BoincManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace BoincManagerWindows.Helpers
{
    public static class StorageHelper
    {
        public static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ApplicationName), Constants.DatabaseFileName)
            };

            var contextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            contextBuilder.UseSqlite(connectionStringBuilder.ConnectionString);
            return contextBuilder.Options;
        }
    }
}
