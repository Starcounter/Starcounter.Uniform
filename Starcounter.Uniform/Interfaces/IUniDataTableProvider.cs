using System.Collections.Generic;

namespace Starcounter.Uniform.Interfaces
{
    public interface IUniDataTableProvider
    {
        IEnumerable<object> SelectRows();
        int CountRows();
        int PageSize { get; set; }
        int Page { get; set; }
    }
}
