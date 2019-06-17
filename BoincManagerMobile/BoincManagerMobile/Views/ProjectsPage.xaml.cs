using BoincManagerMobile.ViewModels;
using System;
using Xamarin.Forms;

namespace BoincManagerMobile.Views
{
    public partial class ProjectsPage : ContentPage
    {
        readonly ProjectsViewModel viewModel;

        public ProjectsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ProjectsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var project = args.SelectedItem as Models.Project;
            if (project == null)
                return;

            await Navigation.PushAsync(new ProjectDetailPage(new ProjectDetailViewModel(project)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddProject_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new AddProjectPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Projects.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}