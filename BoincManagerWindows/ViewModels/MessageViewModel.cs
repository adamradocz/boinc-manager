using System.Collections.Generic;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class MessageViewModel : IFilterableViewModel
    {
        public string ComputerId { get; }
        public string ComputerName { get; private set; }
        public string Project { get; private set; }
        public string Date { get; private set; }
        public string Message { get; private set; }
        public string Priority { get; private set; }

        public Message RpcMessage { get; private set; }

        public MessageViewModel(string computerId)
        {
            ComputerId = computerId;
        }

        public void Update(Message message, string computerName)
        {
            RpcMessage = message;

            Project = message.Project;
            Date = message.Timestamp.ToLocalTime().ToString("g");
            Message = message.Body;
            Priority = message.Priority.ToString();

            ComputerName = computerName;
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return Project;
            yield return Date;
            yield return Message;
        }
    }
}
