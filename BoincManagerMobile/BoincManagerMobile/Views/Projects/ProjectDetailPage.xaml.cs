using System.ComponentModel;
using Xamarin.Forms;
using BoincManagerMobile.ViewModels;
using System;

namespace BoincManagerMobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ProjectDetailPage : ContentPage
    {
        readonly ProjectDetailViewModel _viewModel;

        public ProjectDetailPage(ProjectDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;
        }

        async void RemoveProject_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}