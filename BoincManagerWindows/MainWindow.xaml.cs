using BoincManager.ViewModels;
using BoincManagerWindows.ViewModels;
using BoincRpc;
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Constructor is too early for setting these properties.
            computersDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in HostViewModel.GetLiveFilteringProperties())
            {
                computersDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }

            projectsDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in ProjectViewModel.GetLiveFilteringProperties())
            {
                projectsDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }

            tasksDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in TaskViewModel.GetLiveFilteringProperties())
            {
                tasksDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }

            transfersDataGrid.Items.IsLiveFiltering = true;
            foreach (string propertie in TransferViewModel.GetLiveFilteringProperties())
            {
                transfersDataGrid.Items.LiveFilteringProperties.Add(propertie);
            }            
            
            await _viewModel.StartBoincManager();
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

            ProjectViewModel selectedProject = (ProjectViewModel)projectsDataGrid.SelectedItem;

            if (projectsDataGrid.SelectedItems.Count != 1 || selectedProject.Project.GuiUrls.Count == 0)
            {
                webMenuItem.Visibility = Visibility.Collapsed;
                return;
            }

            foreach (GuiUrl guiUrl in selectedProject.Project.GuiUrls)
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
            if (computersTreeView.SelectedItem is HostViewModel)
            {
                _viewModel.SelectedComputerInTreeView = (HostViewModel)computersTreeView.SelectedItem;
            }
            else
            {
                _viewModel.SelectedComputerInTreeView = null;
            }

            _viewModel.Update();
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
            DataGrid filteredDataGrid;

            switch (tabControl.SelectedIndex)
            {
                case 0: // Computers tab
                    filteredDataGrid = computersDataGrid;
                    break;
                case 1: // Projects tab
                    filteredDataGrid = projectsDataGrid;
                    break;
                case 2: // Tasks tab
                    filteredDataGrid = tasksDataGrid;
                    break;
                case 3: // Transfers tab
                    filteredDataGrid = transfersDataGrid;
                    break;
                case 5: // Messages tab
                    filteredDataGrid = messagesDataGrid;
                    break;
                default:
                    return;
            }
            
            if (string.IsNullOrEmpty(filterTextBox.Text))
            {
                filteredDataGrid.Items.Filter = null;
            }
            else
            {
                filteredDataGrid.Items.Filter = delegate (object item)
                {
                    var filterableViewModel = item as IFilterable;
                    if (filterableViewModel == null)
                        return true;

                    string searchTerms = filterTextBox.Text;                    

                    return filterableViewModel.GetContentsForFiltering().Any(content => content != null && content.IndexOf(searchTerms, StringComparison.InvariantCultureIgnoreCase) != -1);
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
