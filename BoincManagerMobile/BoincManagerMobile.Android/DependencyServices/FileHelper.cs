using System;
using BoincManagerMobile.Droid.DependencyServices;
using BoincManagerMobile.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(FileHelper))]
namespace BoincManagerMobile.Droid.DependencyServices
{
    public class FileHelper : IFileHelper
    {
        public string GetDatabaseFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}