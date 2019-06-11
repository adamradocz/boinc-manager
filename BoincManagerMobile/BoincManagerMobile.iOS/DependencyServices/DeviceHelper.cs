using BoincManagerMobile.Interfaces;
using BoincManagerMobile.iOS.DependencyServices;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceHelper))]
namespace BoincManagerMobile.iOS.DependencyServices
{
    public class DeviceHelper : IDeviceHelper
    {
        public string GetDatabaseFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Database");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }
    }
}