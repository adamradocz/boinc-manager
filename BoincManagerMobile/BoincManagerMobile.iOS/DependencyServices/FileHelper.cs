using BoincManagerMobile.Interfaces;
using BoincManagerMobile.iOS.DependencyServices;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(FileHelper))]
namespace BoincManagerMobile.iOS.DependencyServices
{
    public class FileHelper : IFileHelper
    {
        public string GetDatabaseFolderPath()
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Database");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return libFolder;
        }
    }
}