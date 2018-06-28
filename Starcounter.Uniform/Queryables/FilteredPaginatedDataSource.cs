using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.Generic.Pagination;

namespace Starcounter.Uniform.Queryables
{
    /// <summary>
    /// This is the typical implementation of <see cref="IFilteredDataSource{TViewModel}"/> used when
    /// the source data is a queryable.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public class FilteredPaginatedDataSource<TData, TViewModel> : IFilteredDataSource<TViewModel>
    {
        private readonly IQueryableFilterSorter<TData> _filterSorter;
        private readonly IQueryablePaginator<TData, TViewModel> _paginator;

        private readonly IQueryable<TData> _dataSource;

        // todo: I don't like having this here. I'd like it better to be overridden like ApplyFilter in SorterFilter
        private readonly Func<TData, TViewModel> _converter;
        private FilterOrderConfiguration _filterOrderConfiguration;

        public FilteredPaginatedDataSource(IQueryableFilterSorter<TData> filterSorter,
            IQueryablePaginator<TData, TViewModel> paginator,
            IQueryable<TData> dataSource,
            Func<TData, TViewModel> converter)
        {
            _filterSorter = filterSorter;
            _paginator = paginator;
            _dataSource = dataSource;
            _converter = converter;
        }

        public PaginationConfiguration PaginationConfiguration { get; set; }
        public IReadOnlyCollection<TViewModel> CurrentPageRows { get; private set; }
        public int TotalRows { get; private set; }

        public FilterOrderConfiguration FilterOrderConfiguration
        {
            get => _filterOrderConfiguration;
            set
            {
                _filterOrderConfiguration = value;
                var filteredData = _filterSorter.ApplyFilterAndOrder(_dataSource, value);
                CurrentPageRows = _paginator.GetRows(filteredData, PaginationConfiguration, _converter);
                TotalRows = _paginator.GetTotalRows(filteredData);
            }
        }
    }
}