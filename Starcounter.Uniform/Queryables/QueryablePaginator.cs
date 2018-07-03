using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Uniform.Generic.Pagination;

namespace Starcounter.Uniform.Queryables
{
    public class QueryablePaginator<TData, TViewModel> : IQueryablePaginator<TData, TViewModel>
    {
        public IReadOnlyCollection<TViewModel> GetRows(IQueryable<TData> data,
            PaginationConfiguration paginationConfiguration,
            Converter<TData, TViewModel> converter)
        {
            return data
                .Skip(paginationConfiguration.PageSize * paginationConfiguration.CurrentPageIndex)
                .Take(paginationConfiguration.PageSize)
                .AsEnumerable()
                .Select(dataRow => converter(dataRow))
                .ToList();
        }

        public int GetTotalRows(IQueryable<TData> data)
        {
            return data.Count();
        }
    }
}