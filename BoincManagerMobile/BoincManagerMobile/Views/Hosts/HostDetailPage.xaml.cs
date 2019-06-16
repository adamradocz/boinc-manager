using System.ComponentModel;
using Xamarin.Forms;

using BoincManagerMobile.ViewModels;
using System;
using BoincManager.Models;

namespace BoincManagerMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class HostDetailPage : ContentPage
    {
        readonly HostDetailViewModel _viewModel;

        public HostDetailPage(HostDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;
        }

        async void RemoveHost_Clicked(object sender, EventArgs e)
        {
            using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                var hostConnection = await context.Host.FindAsync(_viewModel.Host.Id);
                if (hostConnection != null)
                {
                    context.Host.Remove(hostConnection);
                    await context.SaveChangesAsync();

                    App.Manager.RemoveHost(_viewModel.Host.Id);

                    MessagingCenter.Send(this, "RemoveHost", _viewModel.Host);
                }
            }

            await Navigation.PopAsync();
        }

    }
}