using BoincManagerMobile.Models;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = nameof(MenuItemType.About);

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}