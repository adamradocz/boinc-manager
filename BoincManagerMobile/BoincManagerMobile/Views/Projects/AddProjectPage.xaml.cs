using System;
using System.ComponentModel;
using Xamarin.Forms;
using BoincManagerMobile.ViewModels;

namespace BoincManagerMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class AddProjectPage : ContentPage
    {
        private readonly AddProjectViewModel _viewModel;

        public AddProjectPage()
        {
            InitializeComponent();

            Title = "Add Project";

            BindingContext = _viewModel = new AddProjectViewModel();

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            //App.Manager.AddHost(HostConnection);            

            //MessagingCenter.Send(this, "AddProject", new Host(App.Manager.GetHostState(HostConnection.Id)));
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}