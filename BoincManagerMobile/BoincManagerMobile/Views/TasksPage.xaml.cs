using BoincManagerMobile.ViewModels;
using Xamarin.Forms;

namespace BoincManagerMobile.Views
{
    public partial class TasksPage : ContentPage
    {
        readonly TasksViewModel viewModel;

        public TasksPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new TasksViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var task = args.SelectedItem as Models.Task;
            if (task == null)
                return;

            await Navigation.PushAsync(new TaskDetailPage(new TaskDetailViewModel(task)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Tasks.Count == 0)
                viewModel.LoadTasksCommand.Execute(null);
        }
    }
}