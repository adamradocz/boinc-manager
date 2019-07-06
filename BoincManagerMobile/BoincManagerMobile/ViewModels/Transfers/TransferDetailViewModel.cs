using BoincManager.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TransferDetailViewModel : BaseViewModel
    {
        public ObservableTransfer Transfer { get; set; }
        public TransferDetailViewModel(ObservableTransfer transfer)
        {
            Title = transfer.FileName;
            Transfer = transfer;
        }
    }
}
