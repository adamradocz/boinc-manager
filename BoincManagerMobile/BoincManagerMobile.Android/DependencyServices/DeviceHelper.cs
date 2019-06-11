using System;
using System.IO;
using BoincManagerMobile.Droid.DependencyServices;
using BoincManagerMobile.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceHelper))]
namespace BoincManagerMobile.Droid.DependencyServices
{
    public class DeviceHelper : IDeviceHelper
    {
        public string GetDatabaseFilePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
        }
    }
}