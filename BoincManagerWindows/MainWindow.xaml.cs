using BoincManager.Interfaces;
using BoincManager.Models;
using BoincManagerWindows.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BoincManagerWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        bool disposed = false;

        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            messagesDataGrid.ItemContainerGenerator.StatusChanged += MessagesDataGrid_ItemContainerGenerator_StatusChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Constructor is too early for setting these properties.

            // The live sorting/grouping/filtering feature is a very expensive one so be careful to activate it only if needed
            // and only the one you actually need. It may be a good idea too to de-activate it when it’s no more needed.
            computersDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in ObservableHost.GetLiveFilteringProperties())
            {
                computersDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }

            projectsDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in ObservableProject.GetLiveFilteringProperties())
            {
                projectsDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }

            tasksDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in ObservableTask.GetLiveFilteringProperties())
            {
                tasksDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }

            transfersDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in ObservableTransfer.GetLiveFilteringProperties())
            {
                transfersDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }
        }

        private void MessagesDataGrid_ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (messagesDataGrid.ItemContainerGenerator.Status == GeneratorStatus.GeneratingContainers)
            {
                messagesDataGrid.ItemContainerGenerator.StatusChanged -= MessagesDataGrid_ItemContainerGenerator_StatusChanged;

                messagesDataGrid.ScrollIntoView(_viewModel.Messages.LastOrDefault());
            }
        }

        private void ProjectContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu projectContextMenu = (ContextMenu)sender;

            MenuItem webMenuItem = projectContextMenu.Items.Cast<MenuItem>().Last();

            webMenuItem.Items.Clear();

            ObservableProject selectedProject = (ObservableProject)projectsDataGrid.SelectedItem;

            if (projectsDataGrid.SelectedItems.Count != 1 || selectedProject.RpcProject.GuiUrls.Count == 0)
            {
                webMenuItem.Visibility = Visibility.Collapsed;
                return;
            }

            foreach (BoincRpc.GuiUrl guiUrl in selectedProject.RpcProject.GuiUrls)
            {
                MenuItem urlMenuItem = new MenuItem
                {
                    Header = guiUrl.Name,
                    ToolTip = guiUrl.Description,
                    Tag = guiUrl.Url
                };
                urlMenuItem.Click += delegate (object s, RoutedEventArgs a) { Process.Start((string)((MenuItem)s).Tag); };

                webMenuItem.Items.Add(urlMenuItem);
            }

            webMenuItem.Visibility = Visibility.Visible;
        }

        private void ComputersTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataGrid filteredDataGrid = GetFilteredDataGrid(tabControl.SelectedIndex);
            if (filteredDataGrid == null)
                return;

            if (computersTreeView.SelectedItem is ObservableHost selectedHost)
            {
                filteredDataGrid.Items.Filter = delegate (object item)
                {
                    switch (item)
                    {
                        case ObservableHost host:
                            return host.Id == selectedHost.Id;
                        case ObservableProject project:
                            return project.HostId == selectedHost.Id;
                        case ObservableTask task:
                            return task.HostId == selectedHost.Id;
                        case ObservableTransfer transfer:
                            return transfer.HostId == selectedHost.Id;
                        case ObservableMessage message:
                            return message.HostId == selectedHost.Id;
                        default:
                            return false;
                    }
                };
            }
            else
            {
                filteredDataGrid.Items.Filter = null;
            }
        }

        private DataGrid GetFilteredDataGrid(int tabControlSelectedIndex)
        {
            switch (tabControlSelectedIndex)
            {
                case 0: // Computers tab
                    return computersDataGrid;
                case 1: // Projects tab
                    return projectsDataGrid;
                case 2: // Tasks tab
                    return tasksDataGrid;
                case 3: // Transfers tab
                    return transfersDataGrid;
                case 5: // Messages tab
                    return messagesDataGrid;
                default:
                    return null;
            }
        }

        private void ComputersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedComputers = computersDataGrid.SelectedItems;
        }
        
        private void ProjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedProjects = projectsDataGrid.SelectedItems;
        }

        private void TasksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedTasks = tasksDataGrid.SelectedItems;
        }

        private void TransfersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedTransfers = transfersDataGrid.SelectedItems;
        }

        private void MessagesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedMessages = messagesDataGrid.SelectedItems;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != tabControl)
                return;

            filterTextBox.Clear();

            computersDataGrid.Items.Filter = null;
            projectsDataGrid.Items.Filter = null;
            tasksDataGrid.Items.Filter = null;
            transfersDataGrid.Items.Filter = null;
            messagesDataGrid.Items.Filter = null;
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataGrid filteredDataGrid = GetFilteredDataGrid(tabControl.SelectedIndex);
            if (filteredDataGrid == null)
                return;


            if (string.IsNullOrEmpty(filterTextBox.Text))
            {
                filteredDataGrid.Items.Filter = null;
            }
            else
            {
                filteredDataGrid.Items.Filter = delegate (object item)
                {
                    var filterable = item as IFilterable;
                    if (filterable == null)
                        return false;

                    string searchTerms = filterTextBox.Text;
                    return filterable.GetContentsForFiltering().Any(
                        content => content != null
                        && content.IndexOf(searchTerms, StringComparison.InvariantCultureIgnoreCase) != -1);
                };
            }
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                filterTextBox.Clear();
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _viewModel.Dispose();
            }

            disposed = true;
        }
    }
}
