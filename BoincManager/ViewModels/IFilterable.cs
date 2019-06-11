using System.Collections.Generic;

namespace BoincManager.ViewModels
{
    public interface IFilterable
    {
        IEnumerable<string> GetContentsForFiltering();
    }
}
