using System.Linq;
using Starcounter.Uniform.Generic.FilterAndSort;

namespace Starcounter.Uniform.Queryables
{
    public interface IQueryableFilterSorter<TData>
    {
        IQueryable<TData> ApplyFilterAndOrder(IQueryable<TData> data, FilterOrderConfiguration configuration);
    }
}