using BoincManager.Models;
using BoincManagerMobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class PreferencesViewModel : BaseViewModel
    {
        public ObservableCollection<ObservableHost> Hosts { get => App.Manager.Hosts; }
        public ObservableHost SelectedHost { get; set; }
        public HostState HostState { get; set; }

        double cpuUsageLimit;
        public double CpuUsageLimit
        {
            get { return cpuUsageLimit; }
            set { SetProperty(ref cpuUsageLimit, value); }
        }

        bool runIfUserActive;
        public bool RunIfUserActive
        {
            get { return runIfUserActive; }
            set { SetProperty(ref runIfUserActive, value); }
        }

        public Command SaveGlobalPreferencesOverrideCommand { get; }

        public PreferencesViewModel()
        {
            Title = nameof(MenuItemType.Preferences);

            SaveGlobalPreferencesOverrideCommand = new Command(async () => await ExecuteSaveGlobalPreferencesOverrideCommand(), CanExecuteSaveGlobalPreferencesOverrideCommand);
        }

        private async Task ExecuteSaveGlobalPreferencesOverrideCommand()
        {
            List<XElement> globalPreferencesOverride = new List<XElement>
            {
                HostState.RpcClient.CreateGlobalPreferencesOverrideElement(BoincRpc.GlobalPreferencesOverrideBoolElement.RunIfUserActive, RunIfUserActive),
                HostState.RpcClient.CreateGlobalPreferencesOverrideElement(BoincRpc.GlobalPreferencesOverrideDoubleElement.CpuUsageLimit, CpuUsageLimit)
            };

            await HostState.RpcClient.SetGlobalPreferencesOverrideAsync(globalPreferencesOverride);
            await HostState.RpcClient.ReadGlobalPreferencesOverrideAsync();
        }

        private bool CanExecuteSaveGlobalPreferencesOverrideCommand()
        {
            return HostState != null;
        }

        public void Update()
        {
            if (SelectedHost == null)            
                throw new NullReferenceException(nameof(SelectedHost));

            HostState = App.Manager.GetHostState(SelectedHost.Id);
            CpuUsageLimit = HostState.BoincState.CoreClientState.GlobalPreferences.CpuUsageLimit;
            RunIfUserActive = HostState.BoincState.CoreClientState.GlobalPreferences.RunIfUserActive;
        }
    }
}