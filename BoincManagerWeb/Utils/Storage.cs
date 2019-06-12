using BoincManager;
using System;
using System.IO;

namespace BoincManagerWeb.Utils
{
    public static class Storage
    {
        public static string GetAppDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ApplicationName);
        }
    }
}
