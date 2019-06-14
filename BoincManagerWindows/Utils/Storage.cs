using BoincManager;
using BoincManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace BoincManagerWindows.Utils
{
    public static class Storage
    {
        public static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ApplicationName, Constants.DatabaseFileName)
            };

            var contextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            contextBuilder.UseSqlite(connectionStringBuilder.ConnectionString);
            return contextBuilder.Options;
        }

        public static string GetAppDataFolderPath()
        {
            var appDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ApplicationName);

            if (!Directory.Exists(appDataFolderPath))
            {
                Directory.CreateDirectory(appDataFolderPath);
            }

            return appDataFolderPath;
        }
    }
}
