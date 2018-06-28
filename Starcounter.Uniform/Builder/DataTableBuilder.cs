using System;
using System.Linq;
using System.Linq.Expressions;
using Starcounter.Uniform.Queryables;

namespace Starcounter.Uniform.Builder
{
    public class DataTableBuilder<TData, TViewModel>
        where TViewModel : Json, new()
    {
        private readonly IQueryable<TData> _dataSource;
        private IQueryableFilterSorter<TData> _filterSorter;
        private Func<TData, TViewModel> _converter;

        public DataTableBuilder(IQueryable<TData> dataSource)
        {
            _dataSource = dataSource;
            _filterSorter = new QueryableFilterSorter<TData>();
            _converter = DefaultConverter;
        }

        private TViewModel DefaultConverter(TData data)
        {
            return new TViewModel
            {
                Data = data
            };
        }

        public DataTableBuilder<TData, TViewModel> SetSorterFilter(IQueryableFilterSorter<TData> filterSorter)
        {
            _filterSorter = filterSorter;
            return this;
        }

        public DataTableBuilder<TData, TViewModel> SetConverter(Func<TData, TViewModel> converter)
        {
            _converter = converter;
            return this;
        }

        // todo This API should be a little bit prettier, introduce ColumnBuilder
        public DataTableBuilder<TData, TViewModel> AddColumn<TColumn>(
            Expression<Func<TViewModel, TColumn>> propertySelector,
            bool isSortable,
            bool isFilterable,
            string displayName
        )
        {
            string propertyName = "get it from the propertySelector";
            return AddColumn(propertyName, isSortable, isFilterable, displayName);
        }

        public DataTableBuilder<TData, TViewModel> AddColumn(
            string propertyName,
            bool isSortable,
            bool isFilterable,
            string displayName
        )
        {
            throw new NotSupportedException();
        }

        public Json Build()
        {
            // bind this to the UI
            new FilteredPaginatedDataSource<TData, TViewModel>(_filterSorter,
                new QueryablePaginator<TData, TViewModel>(),
                _dataSource,
                _converter
            );

            throw new NotSupportedException();
        }

    }
}