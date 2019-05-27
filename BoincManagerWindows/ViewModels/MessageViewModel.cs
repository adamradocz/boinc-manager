using System.Collections.Generic;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class MessageViewModel : BindableBase, IFilterableViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; }
        public string Date { get; }
        public string Message { get; }
        public string Priority { get; }

        public Message RpcMessage { get; private set; }

        public MessageViewModel(HostState hostState, Message message)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            RpcMessage = message;

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
