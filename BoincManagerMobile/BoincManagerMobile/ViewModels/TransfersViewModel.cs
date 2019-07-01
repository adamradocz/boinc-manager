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
        private ObservableCollection<Transfer> _transfers;
        public ObservableCollection<Transfer> Transfers { get => _transfers; set => _transfers = value; }
        public Command LoadTransfersCommand { get; }

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
                GetTransfers(App.Manager.GetAllHostStates(), string.Empty, ref _transfers);
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

        public void GetTransfers(IEnumerable<HostState> hostStates, string searchString, ref ObservableCollection<Transfer> transfers)
        {
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
        }

    }
}