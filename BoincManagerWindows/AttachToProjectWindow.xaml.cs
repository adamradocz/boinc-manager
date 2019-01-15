using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BoincRpc;

namespace BoincManagerWindows
{
    public partial class AttachToProjectWindow : Window
    {
        public AttachToProjectWindow(RpcClient rpcClient)
        {
            InitializeComponent();

            FetchAllProjectsList(rpcClient);
        }

        private async void FetchAllProjectsList(RpcClient rpcClient)
        {
            listViewAllProjectList.ItemsSource = await rpcClient.GetAllProjectsListAsync();

            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(listViewAllProjectList.ItemsSource);

            PropertyGroupDescription groupDescription = new PropertyGroupDescription("GeneralArea");
            groupDescription.StringComparison = StringComparison.InvariantCultureIgnoreCase;
            myView.GroupDescriptions.Add(groupDescription);

            myView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            textBlockLoading.Visibility = Visibility.Collapsed;
        }

        private void checkBoxGroup_Click(object sender, RoutedEventArgs e)
        {
            CollectionView myView = (CollectionView)CollectionViewSource.GetDefaultView(listViewAllProjectList.ItemsSource);
            
            if (checkBoxGroup.IsChecked == false)
                myView.GroupDescriptions.Clear();
            else
            {
                PropertyGroupDescription groupDescription = new PropertyGroupDescription("GeneralArea");
                groupDescription.StringComparison = StringComparison.InvariantCultureIgnoreCase;
                myView.GroupDescriptions.Add(groupDescription);
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void buttonContinue_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(textBoxUrl.Text);
        }

        private void textBlockWebsite_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ProjectListEntry ple = listViewAllProjectList.SelectedItem as ProjectListEntry;

            if (ple == null)
                return;

            Process.Start(ple.Url);
        }
    }
}
