using System.Linq;
using Starcounter.Uniform.Generic.FilterAndSort;

namespace Starcounter.Uniform.Queryables
{
    public interface IQueryableFilter<TData>
    {
        IQueryable<TData> Apply(IQueryable<TData> data, FilterOrderConfiguration configuration);
    }
}