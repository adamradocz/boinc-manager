using System.Collections.Generic;
using System.Threading.Tasks;
using BoincRpc;

namespace BoincManager.Models
{
    public class BoincState
    {
        private readonly RpcClient RpcClient;

        public CoreClientStatus CoreClientStatus { get; private set; }
        public CoreClientState CoreClientState { get; private set; }

        public IReadOnlyList<Project> Projects { get; private set; }
        public IReadOnlyList<App> Apps { get; private set; }
        public IReadOnlyList<AppVersion> AppVersions { get; private set; }
        public IReadOnlyList<Workunit> Workunits { get; private set; }
        
        // Tasks
        public IReadOnlyList<Result> Results { get; private set; }
        public IReadOnlyList<FileTransfer> FileTransfers { get; private set; }

        public List<Message> Messages { get; private set; }

        private int lastMessageNumber = 0;

        public BoincState(RpcClient rpcClient)
        {
            RpcClient = rpcClient;
            Messages = new List<Message>();
        }

        /// <summary>
        /// Update the CoreClientState(Projects/Apps/AppVersions/Workunits/Results), CoreClientStatus, FileTransfers, Messages
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAll()
        {
            await UpdateCoreClientStatus();
            await UpdateCoreClientState();
            await UpdateFileTransfers();
            await UpdateMessages();
        }

        public async Task UpdateCoreClientStatus()
        {
            CoreClientStatus = await RpcClient.GetCoreClientStatusAsync();
        }

        /// <summary>
        /// Updates the CoreClientState, plus the Projects/Apps/AppVersions/Workunits/Results Lists.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateCoreClientState()
        {
            CoreClientState = await RpcClient.GetStateAsync();

            Projects = CoreClientState.Projects;
            Apps = CoreClientState.Apps;
            AppVersions = CoreClientState.AppVersions;
            Workunits = CoreClientState.Workunits;
            Results = CoreClientState.Results;            
        }

        public async Task UpdateProjects()
        {
            Projects = await RpcClient.GetProjectStatusAsync();
        }

        public async Task UpdateResults()
        {
            Results = await RpcClient.GetResultsAsync();
        }

        public async Task UpdateFileTransfers()
        {
            FileTransfers = await RpcClient.GetFileTransfersAsync();
        }

        public async Task UpdateMessages()
        {
            ObservableMessage[] newMessages = await RpcClient.GetMessagesAsync(lastMessageNumber);
            lastMessageNumber += newMessages.Length;
            Messages.AddRange(newMessages);
        }   
    }
}
