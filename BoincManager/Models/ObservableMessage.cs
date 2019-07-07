using System.Collections.Generic;
using BoincManager.Interfaces;

namespace BoincManager.Models
{
    public class ObservableMessage : BindableBase, IMessage, IFilterable
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; }
        public string Date { get; }
        public string MessageBody { get; }
        public string Priority { get; }

        public BoincRpc.Message RpcMessage { get; private set; }

        public ObservableMessage(HostState hostState, BoincRpc.Message rpcMessage)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            RpcMessage = rpcMessage;

            Project = rpcMessage.Project;
            Date = rpcMessage.Timestamp.ToLocalTime().ToString("g");
            MessageBody = rpcMessage.Body;
            Priority = rpcMessage.Priority.ToString();
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return HostName;
            yield return Project;
            yield return Date;
            yield return MessageBody;
        }
    }
}
