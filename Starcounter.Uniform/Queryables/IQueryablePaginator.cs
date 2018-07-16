using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Uniform.Generic.Pagination;

namespace Starcounter.Uniform.Queryables
{
    /// <summary>
    /// Its only sensible implementation is <see cref="QueryablePaginator{TData,TViewModel}"/>.
    /// This interface exists to facilitate reuse of paging code and should be primarily used by
    /// the infrastructure code, not by app developers directly.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IQueryablePaginator<TData, TViewModel>
    {
        IReadOnlyCollection<TViewModel> GetRows(
            IQueryable<TData> data,
            PaginationConfiguration paginationConfiguration,
            Converter<TData, TViewModel> converter);

        int GetTotalRows(IQueryable<TData> data);
    }
}