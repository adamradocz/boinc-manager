using System.Collections.ObjectModel;

using Xamarin.Forms;
using BoincManager.Models;
using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TransfersViewModel : BaseViewModel
    {
        public ObservableCollection<ObservableTransfer> Transfers { get => App.Manager.Transfers; }
        public Command RefreshTransfersCommand { get; }

        public TransfersViewModel()
        {
            Title = nameof(MenuItemType.Transfers);

            RefreshTransfersCommand = new Command(async() => await ExecuteRefreshTransfersCommand());
        }

        async System.Threading.Tasks.Task ExecuteRefreshTransfersCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await App.Manager.Update();
            IsBusy = false;
        }
    }
}