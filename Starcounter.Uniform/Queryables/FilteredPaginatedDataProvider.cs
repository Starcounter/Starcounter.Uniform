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
    public sealed class FilteredPaginatedDataProvider<TData, TViewModel> : IFilteredDataProvider<TViewModel>
        where TViewModel: Json
    {
        private readonly IQueryableFilter<TData> _filter;
        private readonly IQueryablePaginator<TData, TViewModel> _paginator;

        private readonly IQueryable<TData> _dataSource;

        private readonly Converter<TData, TViewModel> _converter;
        private IReadOnlyCollection<TViewModel> _currentPageRows;
        private bool _isDisposed = false;
        private IQueryable<TData> _filteredData;

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

        /// <inheritdoc />
        public PaginationConfiguration PaginationConfiguration { get; set; }

        /// <inheritdoc />
        public FilterOrderConfiguration FilterOrderConfiguration { get; set; }

        // TODO: _paginator should be called only after pagination or filter has been changed
        /// <inheritdoc />
        public IReadOnlyCollection<TViewModel> CurrentPageRows
        {
            get
            {
                CheckDisposed();
                ApplyFilters();
                DisposeOfRows();
                _currentPageRows = _paginator.GetRows(_filteredData, PaginationConfiguration, _converter);
                return _currentPageRows;
            }
        }

        /// <inheritdoc />
        public int TotalRows
        {
            get
            {
                CheckDisposed();
                ApplyFilters();
                return _paginator.GetTotalRows(_filteredData);
            }
        }

        /// <summary>
        /// Calls <see cref="IDisposable.Dispose"/> on all of currently held rows that implement <see cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            DisposeOfRows();
            _isDisposed = true;
        }

        private void ApplyFilters()
        {
            _filteredData = _filter.Apply(_dataSource, FilterOrderConfiguration);
        }

        private void DisposeOfRows()
        {
            if (_currentPageRows != null)
            {
                foreach (var currentPageRow in _currentPageRows)
                {
                    currentPageRow.Dispose();
                }
            }
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}