using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Uniform.Generic.Pagination;

namespace Starcounter.Uniform.Queryables
{
    /// <summary>
    /// This is the typical implementation of <see cref="IPaginatedDataProvider{TViewModel}"/> used when
    /// the data source is a queryable.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TViewModel"></typeparam>
    public sealed class PaginatedQueryableDataProvider<TData, TViewModel> : IPaginatedDataProvider<TViewModel>
        where TViewModel : Json, new()
    {
        private readonly IQueryable<TData> _queryable;
        private readonly Converter<TData, TViewModel> _converter;
        private readonly IQueryablePaginator<TData, TViewModel> _paginator;
        private IReadOnlyCollection<TViewModel> _currentRows;
        private bool _isDisposed;

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

        public IReadOnlyCollection<TViewModel> CurrentPageRows
        {
            get
            {
                CheckDisposed();
                DisposeOfRows();
                _currentRows = _paginator.GetRows(_queryable, PaginationConfiguration, _converter);
                return _currentRows;
            }
        }
        public int TotalRows => _paginator.GetTotalRows(_queryable);

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            DisposeOfRows();
            _isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void DisposeOfRows()
        {
            if (_currentRows != null)
            {
                foreach (var currentRow in _currentRows)
                {
                    (currentRow as IDisposable)?.Dispose();
                }
            }
        }
    }
}