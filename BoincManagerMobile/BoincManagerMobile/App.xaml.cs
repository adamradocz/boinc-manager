using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BoincManagerMobile.Services;
using BoincManagerMobile.Views;
using BoincManagerMobile.Interfaces;

namespace BoincManagerMobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            var a = DependencyService.Get<IDeviceHelper>().GetDatabaseFilePath("db");

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
