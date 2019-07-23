using BoincManagerMobile.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BoincManagerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreferencesPage : ContentPage
    {
        readonly PreferencesViewModel viewModel;

        public PreferencesPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new PreferencesViewModel();
        }

        private void HostPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            viewModel.Update();
            viewModel.SaveGlobalPreferencesOverrideCommand.ChangeCanExecute();
        }

        private void CPULimitSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            int stepValue = 10;
            ((Slider)sender).Value = Math.Round(e.NewValue / stepValue) * stepValue;
        }
    }
}