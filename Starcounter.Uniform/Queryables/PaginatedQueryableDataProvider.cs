using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Uniform.Generic.Pagination;

namespace Starcounter.Uniform.Queryables
{
    /// <summary>
    /// This is the typical implementation of <see cref="IPaginatedDataProvider{TViewModel}"/> used when
    /// the source data is a queryable.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public class PaginatedQueryableDataProvider<TData, TViewModel> : IPaginatedDataProvider<TViewModel>
        where TViewModel : Json, new()
    {
        private readonly IQueryable<TData> _queryable;
        private readonly Converter<TData, TViewModel> _converter;
        private readonly IQueryablePaginator<TData, TViewModel> _paginator;

        public PaginatedQueryableDataProvider(
            IQueryable<TData> queryable,
            Converter<TData, TViewModel> converter,
            IQueryablePaginator<TData, TViewModel> paginator)
        {
            _queryable = queryable;
            _converter = converter;
            _paginator = paginator;
        }

        public PaginationConfiguration PaginationConfiguration { get; set; }

        public IReadOnlyCollection<TViewModel> CurrentPageRows =>
            _paginator.GetRows(_queryable, PaginationConfiguration, _converter);

        public int TotalRows => _paginator.GetTotalRows(_queryable);
    }
}