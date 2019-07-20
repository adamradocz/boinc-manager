using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BoincManager;
using BoincManagerMobile.Views;
using BoincManager.Models;

namespace BoincManagerMobile
{
    public partial class App : Application
    {
        public static Manager Manager;

        public App()
        {
            InitializeComponent();

            Manager = new Manager();

            // Initialize the application            
            using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                BoincManager.Utils.InitializeApplication(context, Manager, true);
            }

            System.Threading.Tasks.Task.Run(() => Manager.Start());

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
