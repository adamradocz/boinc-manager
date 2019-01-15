using BoincManager.Models;
using BoincRpc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BoincManager
{
    public static class Utils
    {        
        /// <summary>
        /// Read the Host data from the file. If the file doesn't exist or empty, it creates the file with default values and returns it.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>List of Host authorization data.</returns>
        public static List<HostConnectionModel> ReadHostsDataFromFile(string fileName)
        {
            var data = new List<HostConnectionModel>();

            // Check whether the file is exist or empty.
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists || fileInfo.Length == 0)
            {
                data.Add(GetDefaultHostConnection());
                WriteHostsConnectionDataToFile(data, fileName);
                return data;
            }

            // Read the file.
            using (var reader = new StreamReader(fileName))
            {
                XmlSerializer deserializer = new XmlSerializer(data.GetType());
                data = (List<HostConnectionModel>) deserializer.Deserialize(reader);
            }

            // The file wasn't empty but didn't contain host connection data.
            if (data.Count == 0)
            {
                data.Add(GetDefaultHostConnection());
                WriteHostsConnectionDataToFile(data, fileName);
            }

            return data;
        }

        private static HostConnectionModel GetDefaultHostConnection()
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

            return new HostConnectionModel() { Name = Environment.MachineName, IpAddress = "localhost", Port = Constants.BoincDefaultPort, Password = password };
        }
        
        /// <summary>
        /// Write the Host data to a file.
        /// </summary>
        /// <param name="hostConnectionData">List of host connection data.</param>
        /// <param name="fileName">Filename</param>
        public static void WriteHostsConnectionDataToFile(IEnumerable<HostConnectionModel> hostConnectionData, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(hostConnectionData.GetType());
                serializer.Serialize(writer, hostConnectionData);
            }
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
