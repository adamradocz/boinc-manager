using System.Collections.Generic;

namespace BoincManager.Interfaces
{
    public interface IFilterable
    {
        IEnumerable<string> GetContentsForFiltering();
    }
}
