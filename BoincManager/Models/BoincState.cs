using System.Collections.Generic;
using System.Threading.Tasks;
using BoincRpc;

namespace BoincManager.Models
{
    public class BoincState
    {
        readonly RpcClient rpcClient;

        public CoreClientStatus CoreClientStatus { get; private set; }
        public CoreClientState CoreClientState { get; private set; }

        public IReadOnlyList<Project> Projects { get; private set; }
        public IReadOnlyList<App> Apps { get; private set; }
        public IReadOnlyList<AppVersion> AppVersions { get; private set; }
        public IReadOnlyList<Workunit> Workunits { get; private set; }
        public IReadOnlyList<Result> Results { get; private set; }
        public IReadOnlyList<FileTransfer> FileTransfers { get; private set; }

        private int lastMessageNumber = 0;

        public BoincState(RpcClient rpcClient)
        {
            this.rpcClient = rpcClient;
        }

        /// <summary>
        /// Update the CoreClientState(Projects/Apps/AppVersions/Workunits/Results), CoreClientStatus, FileTransfers
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAll()
        {
            await UpdateCoreClientStatus();
            await UpdateCoreClientState();
            await UpdateFileTransfers();
        }

        public async Task UpdateCoreClientStatus()
        {
            CoreClientStatus = await rpcClient.GetCoreClientStatusAsync();
        }

        /// <summary>
        /// Updates the CoreClientState, plus the Projects/Apps/AppVersions/Workunits/Results Lists.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateCoreClientState()
        {
            CoreClientState = await rpcClient.GetStateAsync();

            Projects = CoreClientState.Projects;
            Apps = CoreClientState.Apps;
            AppVersions = CoreClientState.AppVersions;
            Workunits = CoreClientState.Workunits;
            Results = CoreClientState.Results;
        }

        public async Task UpdateProjects()
        {
            Projects = await rpcClient.GetProjectStatusAsync();
        }

        public async Task UpdateResults()
        {
            Results = await rpcClient.GetResultsAsync();
        }

        public async Task UpdateFileTransfers()
        {
            FileTransfers = await rpcClient.GetFileTransfersAsync();
        }

        public async Task<Message[]> GetNewMessages()
        {
            Message[] newMessages = await rpcClient.GetMessagesAsync(lastMessageNumber);
            lastMessageNumber += newMessages.Length;
        
            return newMessages;
        }
    }
}
