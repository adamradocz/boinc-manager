using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

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

        public void OnGet()
        {
            Transfers = GetTransfers(_manager.GetAllHostStates());
        }

        private List<TransferViewModel> GetTransfers(IEnumerable<HostState> hostStates)
        {
            var transfers = new List<TransferViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
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
