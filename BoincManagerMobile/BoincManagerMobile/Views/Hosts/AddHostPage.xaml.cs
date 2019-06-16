using System;
using System.ComponentModel;
using Xamarin.Forms;

using BoincManagerMobile.Models;
using BoincManager.Models;

namespace BoincManagerMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AddHostPage : ContentPage
    {
        public HostConnection HostConnection { get; set; }

        public AddHostPage()
        {
            InitializeComponent();

            Title = "Add Host";

            HostConnection = new HostConnection();

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                await context.AddAsync(HostConnection);
                await context.SaveChangesAsync();
            }

            App.Manager.AddHost(HostConnection);
            if (HostConnection.AutoConnect)
            {
                await App.Manager.Connect(HostConnection.Id);
            }

            MessagingCenter.Send(this, "AddHost", new Host(App.Manager.GetHostState(HostConnection.Id)));
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}