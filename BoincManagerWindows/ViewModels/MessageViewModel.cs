using System.Collections.Generic;
using BoincManager.Models;

namespace BoincManagerWindows.ViewModels
{
    class MessageViewModel : BoincManager.ViewModels.MessageViewModel, IFilterableViewModel
    {
        public MessageViewModel(HostState hostState) : base(hostState)
        {
        }
        
        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return HostName;
            yield return Project;
            yield return Date;
            yield return Message;
        }
    }
}
