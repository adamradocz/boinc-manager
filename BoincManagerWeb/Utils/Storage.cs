using BoincManager;
using System;
using System.IO;

namespace BoincManagerWeb.Utils
{
    public static class Storage
    {
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
