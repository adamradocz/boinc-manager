using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BoincManagerMobile.Models;
using BoincManager.Models;
using BoincManager;
using BoincManagerMobile.ViewModels;

namespace BoincManagerMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AddHostPage : ContentPage
    {
        public HostConnection Host { get; set; }

        public AddHostPage()
        {
            InitializeComponent();

            Title = "Add Host";

            Host = new HostConnection();

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                await context.AddAsync(Host);
                await context.SaveChangesAsync();
            }

            App.Manager.AddHost(Host);
            if (Host.AutoConnect)
            {
                await App.Manager.Connect(Host.Id);
            }

            var hostViewModel = new Host(App.Manager.GetHostState(Host.Id));
            MessagingCenter.Send(this, "AddHost", hostViewModel);
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}