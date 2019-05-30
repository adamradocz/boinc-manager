using BoincManager.Models;
using BoincRpc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BoincManager
{
    public static class Utils
    {
        /// <summary>
        /// Ensure everything is set to run the Application.
        /// - Ensure database folder exists.
        /// - Ensure the database is created and up to date.
        /// - Initialize BoincManager
        /// </summary>
        /// <param name="databaseFolderPath"></param>
        public static void InitializeApplication(string databaseFolderPath, ApplicationDbContext context, Manager manager)
        {
            // Ensure the directory and all its parents exist. If it exist, it'll do nothing.
            Directory.CreateDirectory(databaseFolderPath);

            // Ensure the database is created and up to date at the start of the application
            context.Database.Migrate();
            
            manager.Initialize(context.Host.ToList());
        }

        public static string GetApplicationDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ApplicationName);
        }

        public static string GetLocalhostGuiRpcPassword()
        {
            string filePath = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = @"C:\ProgramData\BOINC\gui_rpc_auth.cfg";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                filePath = @"/var/lib/boinc/gui_rpc_auth.cfg";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                filePath = @"/Library/Application Support/BOINC Data/gui_rpc_auth.cfg";
            }

            string password = string.Empty;
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                password = File.ReadAllText(filePath);
            }

            return password;
        }
        
        /// <summary>
        /// Convert the filesize from byte to human-readable format.
        /// </summary>
        /// <param name="length">Filesize in bytes</param>
        /// <returns>Human-readable filesize.</returns>
        public static string ConvertBytesToFileSize(double length)
        {
            string[] sizes = { "{0:F0}B", "{0:F2}KB", "{0:F2}MB", "{0:F2}GB", "{0:F2}TB" };

            int order = 0;

            while (length >= 1024 && order < sizes.Length - 1)
            {
                order++;
                length /= 1024;
            }

            return string.Format(sizes[order], length);
        }

        /// <summary>
        /// Convert the duration to human-readable format.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns>Human-readable duration.</returns>
        public static string ConvertDuration(TimeSpan duration)
        {
            return duration.TotalHours < 24 ? duration.ToString("hh':'mm':'ss") : duration.ToString("d'd 'hh':'mm':'ss");
        }

        public static string ConvertDateTime(DateTimeOffset dateTime)
        {
            return dateTime.ToLocalTime().ToString("g");
        }

        public static string GetTimeRemaining(FileTransfer fileTransfer)
        {
            if (fileTransfer.TransferSpeed <= 0)
                return string.Empty;

            TimeSpan timeRemaining = TimeSpan.FromSeconds((fileTransfer.NumberOfBytes - fileTransfer.BytesTransferred) / fileTransfer.TransferSpeed);

            return $"{timeRemaining:%m} minutes {timeRemaining:%s} seconds";
        }

        // from: https://blogs.msdn.microsoft.com/pfxteam/2012/03/05/implementing-a-simple-foreachasync-part-2/
        /// <summary>
        /// This method is similar in nature to Parallel.ForEach, with the primary difference being that
        /// Parallel.ForEach is a synchronous method and uses synchronous delegates.
        /// async doesn't work with Parallel.ForEach. In particular, that async lambda is being converted to an async void method, which is a big NO.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">List to iterate</param>
        /// <param name="functionBody">Function you want to call.</param>
        /// <returns></returns>
        public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> functionBody)
        {
            // Limit the number of operations that are able to run in parallel.
            // Task.WhenAll() has a tendency to become unperformant with large amount of tasks firing simultaneously - without moderation.
            // Using a partition with a limit on the degree of parallelism based on the number of CPU cores.
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(Environment.ProcessorCount)
                select Task.Run(async delegate {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            await functionBody(partition.Current);
                        }
                    }
                }));            
        }

    }
}
