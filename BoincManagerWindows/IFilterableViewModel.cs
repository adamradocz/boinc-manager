using System.Collections.Generic;

namespace BoincManagerWindows
{
    interface IFilterableViewModel
    {
        IEnumerable<string> GetContentsForFiltering();
    }
}
