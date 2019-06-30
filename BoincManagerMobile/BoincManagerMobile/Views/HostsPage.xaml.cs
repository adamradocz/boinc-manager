using System;
using System.ComponentModel;
using Xamarin.Forms;

using BoincManagerMobile.ViewModels;
using BoincManagerMobile.Models;

namespace BoincManagerMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class HostsPage : ContentPage
    {
        readonly HostsViewModel viewModel;

        public HostsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new HostsViewModel(Navigation);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Host host))
                return;

            await Navigation.PushAsync(new HostDetailPage(new HostDetailViewModel(host, Navigation)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Hosts.Count == 0)
                viewModel.LoadHostsCommand.Execute(null);
        }
    }
}