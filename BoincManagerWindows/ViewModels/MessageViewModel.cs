using System.Collections.Generic;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class MessageViewModel : BindableBase, IFilterableViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; private set; }
        public string Date { get; private set; }
        public string Message { get; private set; }
        public string Priority { get; private set; }

        public MessageViewModel(HostState hostState)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;
        }
        
        public void Update(Message message)
        {
            Project = message.Project;
            Date = message.Timestamp.ToLocalTime().ToString("g");
            Message = message.Body;
            Priority = message.Priority.ToString();
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
