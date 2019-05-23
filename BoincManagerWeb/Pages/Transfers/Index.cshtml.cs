using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoincManagerWeb.Pages.Transfers
{
    public class IndexModel : PageModel
    {
        public readonly BoincManager.Manager _manager;

        public List<TransferViewModel> Transfers { get; set; }

        public IndexModel(BoincManager.Manager manager)
        {
            _manager = manager;
        }

        public async Task OnGetAsync()
        {
            Transfers = await GetTransfersAsync(_manager.GetAllHostState());
        }

        private async Task<List<TransferViewModel>> GetTransfersAsync(IEnumerable<HostState> hostsState)
        {
            List<TransferViewModel> transfers = new List<TransferViewModel>();

            foreach (var hostState in hostsState)
            {
                if (hostState.Connected)
                {
                    await hostState.BoincState.UpdateFileTransfers();

                    foreach (var fileTransfer in hostState.BoincState.FileTransfers)
                    {
                        transfers.Add(new TransferViewModel(hostState, fileTransfer));
                    }
                }
            }

            return transfers;
        }
    }
}
