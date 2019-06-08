using BoincManager.Models;
using BoincManager.ViewModels;
using BoincRpc;
using System.Collections.Generic;

namespace BoincManagerWeb.ViewModels
{
    public class MessageViewModel : IFilterable
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; }
        public string Date { get; }
        public string Message { get; }
        public string Priority { get; }

        public MessageViewModel(HostState hostState, Message message)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

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
