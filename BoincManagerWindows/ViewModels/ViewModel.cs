using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BoincManagerWindows.ViewModels
{
    /// <summary>
    /// The ObservableCollection<T> Class provides notifications only when items get added, removed, or when the entire list is refreshed, but not on update.
    ///  The implementation of INotifyPropertyChange in Model class, will automatically update UI when some propery in ObservableCollection item updated.
    /// </summary>
    abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property.  
        /// The CallerMemberName attribute that is applied to the optional propertyName  
        /// parameter causes the property name of the caller to be substituted as an argument.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The private variable of the property.</param>
        /// <param name="value">The value variable of the property.</param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
