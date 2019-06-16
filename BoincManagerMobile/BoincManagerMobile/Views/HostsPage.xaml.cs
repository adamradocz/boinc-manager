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

            BindingContext = viewModel = new HostsViewModel();
        }

        async void OnHostSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var host = args.SelectedItem as Host;
            if (host == null)
                return;

            await Navigation.PushAsync(new HostDetailPage(new HostDetailViewModel(host)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddHost_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AddHostPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Hosts.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}