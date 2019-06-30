using System.ComponentModel;
using Xamarin.Forms;

using BoincManagerMobile.ViewModels;

namespace BoincManagerMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AddHostPage : ContentPage
    {
        readonly AddHostViewModel _viewModel;

        public AddHostPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new AddHostViewModel(Navigation);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.AddHostCommand.RaiseCanExecuteChanged();
        }
    }
}