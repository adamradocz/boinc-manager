using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xamarin.Forms;
using System.Collections.Generic;
using BoincManager.Models;
using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TransfersViewModel : BaseViewModel
    {
        public ObservableCollection<Transfer> Transfers { get; set; }
        public Command LoadTransfersCommand { get; set; }

        public TransfersViewModel()
        {
            Title = nameof(MenuItemType.Transfers);
            Transfers = new ObservableCollection<Transfer>();

            LoadTransfersCommand = new Command(() => ExecuteLoadTransfersCommand());
        }

        void ExecuteLoadTransfersCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Transfers.Clear();
                var items = GetTransfers(App.Manager.GetAllHostStates(), "");
                foreach (var item in items)
                {
                    Transfers.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public List<Transfer> GetTransfers(IEnumerable<HostState> hostStates, string searchString)
        {
            var transfers = new List<Transfer>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var rpcFileTransfer in hostState.BoincState.FileTransfers)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            transfers.Add(new Transfer(hostState, rpcFileTransfer));
                        }
                        else
                        {
                            var transfer = new Transfer(hostState, rpcFileTransfer);
                            foreach (var content in transfer.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the Models's property
                                    transfers.Add(transfer);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return transfers;
        }

    }
}