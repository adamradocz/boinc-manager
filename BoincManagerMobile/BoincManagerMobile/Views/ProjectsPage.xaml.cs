using BoincManagerMobile.ViewModels;
using Xamarin.Forms;

namespace BoincManagerMobile.Views
{
    public partial class ProjectsPage : ContentPage
    {
        readonly ProjectsViewModel viewModel;

        public ProjectsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ProjectsViewModel(Navigation);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var project = args.SelectedItem as Models.Project;
            if (project == null)
                return;

            await Navigation.PushAsync(new ProjectDetailPage(new ProjectDetailViewModel(project, Navigation)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Projects.Count == 0)
                viewModel.LoadProjectsCommand.Execute(null);
        }
    }
}