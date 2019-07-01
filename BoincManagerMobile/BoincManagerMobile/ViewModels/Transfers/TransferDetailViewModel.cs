using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TransferDetailViewModel : BaseViewModel
    {
        public Transfer Transfer { get; set; }
        public TransferDetailViewModel(Transfer transfer)
        {
            Title = transfer.FileName;
            Transfer = transfer;
        }
    }
}
