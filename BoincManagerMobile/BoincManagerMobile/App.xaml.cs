using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BoincManager;
using BoincManagerMobile.Services;
using BoincManagerMobile.Views;
using BoincManager.Models;

namespace BoincManagerMobile
{
    public partial class App : Application
    {
        public static Manager _manager;

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();

            _manager = new Manager();

            // Initialize the application            
            using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                BoincManager.Utils.InitializeApplication(context, _manager);
            }

            _manager.Start();
            _manager.AddHost(new Host("Nasy", "192.168.0.100", "123"));
            _manager.ConnectAll();

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
