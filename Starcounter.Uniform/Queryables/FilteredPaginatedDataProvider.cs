using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.Generic.Pagination;

namespace Starcounter.Uniform.Queryables
{
    /// <summary>
    /// This is the typical implementation of <see cref="IFilteredDataProvider{TViewModel}"/> used when
    /// the source data is a queryable. Instances of this class can be created using <see cref="DataProviderBuilder{TData,TViewModel}"/> or <see cref="DataTableBuilder{TViewModel}"/>
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public class FilteredPaginatedDataProvider<TData, TViewModel> : IFilteredDataProvider<TViewModel>
    {
        private readonly IQueryableFilter<TData> _filter;
        private readonly IQueryablePaginator<TData, TViewModel> _paginator;

        private readonly IQueryable<TData> _dataSource;

        // todo: I don't like having this here. I'd like it better to be overridden like ApplyFilter in SorterFilter
        private readonly Converter<TData, TViewModel> _converter;
        private IReadOnlyCollection<TViewModel> _currentPageRows;
        private int _totalRows;

        public FilteredPaginatedDataProvider(IQueryableFilter<TData> filter,
            IQueryablePaginator<TData, TViewModel> paginator,
            IQueryable<TData> dataSource,
            Converter<TData, TViewModel> converter)
        {
            _filter = filter;
            _paginator = paginator;
            _dataSource = dataSource;
            _converter = converter;
        }

        public PaginationConfiguration PaginationConfiguration { get; set; }

        public FilterOrderConfiguration FilterOrderConfiguration { get; set; }

        // TODO: Reload rows should be called only after pagination or filter has been changed
        public IReadOnlyCollection<TViewModel> CurrentPageRows
        {
            get
            {
                ReloadRows();
                return _currentPageRows;
            }
        }

        public int TotalRows
        {
            get
            {
                ReloadRows();
                return _totalRows;
            }
        }

        private void ReloadRows()
        {
            var filteredData = _filter.Apply(_dataSource, FilterOrderConfiguration);
            _currentPageRows = _paginator.GetRows(filteredData, PaginationConfiguration, _converter);
            _totalRows = _paginator.GetTotalRows(filteredData);
        }
    }
}