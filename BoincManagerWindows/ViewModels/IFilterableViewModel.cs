using System.Collections.Generic;

namespace BoincManagerWindows.ViewModels
{
    interface IFilterableViewModel
    {
        IEnumerable<string> GetContentsForFiltering();
    }
}
