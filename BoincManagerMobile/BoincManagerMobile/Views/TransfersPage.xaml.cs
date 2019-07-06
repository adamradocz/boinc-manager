using BoincManager.Models;
using BoincManagerMobile.ViewModels;
using Xamarin.Forms;

namespace BoincManagerMobile.Views
{
    public partial class TransfersPage : ContentPage
    {
        readonly TransfersViewModel viewModel;

        public TransfersPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new TransfersViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var transfer = args.SelectedItem as ObservableTransfer;
            if (transfer == null)
                return;

            await Navigation.PushAsync(new TransferDetailPage(new TransferDetailViewModel(transfer)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Transfers.Count == 0)
                viewModel.RefreshTransfersCommand.Execute(null);
        }
    }
}