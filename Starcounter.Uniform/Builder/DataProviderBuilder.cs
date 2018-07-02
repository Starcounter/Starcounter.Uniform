using System;
using System.Linq;
using Starcounter.Uniform.Queryables;

namespace Starcounter.Uniform.Builder
{
    /// <summary>
    /// Provides fluent API to create instances of <see cref="FilteredPaginatedDataSource{TData,TViewModel}"/>
    /// </summary>
    /// <typeparam name="TData">The type of original data, available as a queryable</typeparam>
    /// <typeparam name="TViewModel">The type of view-models that will be exposed by the data source</typeparam>
    public class DataProviderBuilder<TData, TViewModel>
        where TViewModel : Json, new()
    {
        private readonly IQueryable<TData> _queryable;
        private IQueryableFilter<TData> _filter;
        private Func<TData, TViewModel> _converter;

        /// <summary>
        /// Construct new <see cref="DataProviderBuilder{TData,TViewModel}"/> instance
        /// </summary>
        /// <param name="queryable"></param>
        /// <remarks>This class is not intended to be constructed by app developers directly. Rather, they should use it as part of <see cref="DataTableBuilder{TViewModel}"/></remarks>
        public DataProviderBuilder(IQueryable<TData> queryable)
        {
            _queryable = queryable;
            _filter = new QueryableFilter<TData>();
            _converter = DefaultConverter;
        }

        private TViewModel DefaultConverter(TData data)
        {
            return new TViewModel
            {
                Data = data
            };
        }

        /// <summary>
        /// Specify object which will override default support for filtering and ordering data
        /// </summary>
        /// <param name="filter">This object will handle filtering and ordering of data. Usually it's of class derived from <see cref="QueryableFilter{TData}"/></param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataProviderBuilder<TData, TViewModel> WithFilter(IQueryableFilter<TData> filter)
        {
            _filter = filter;
            return this;
        }

        /// <summary>
        /// Specify object which will override default creation of view-models
        /// </summary>
        /// <param name="converter">The delegate that will handle creating view-models, usually with input data bound to Data property.</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataProviderBuilder<TData, TViewModel> WithConverter(Func<TData, TViewModel> converter)
        {
            _converter = converter;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="FilteredPaginatedDataSource{TData,TViewModel}"/>
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method is not intended to be used by app developers directly. Rather, they should use it as part of <see cref="DataTableBuilder{TViewModel}"/></remarks>
        public FilteredPaginatedDataSource<TData, TViewModel> Build()
        {
            return new FilteredPaginatedDataSource<TData, TViewModel>(_filter,
                new QueryablePaginator<TData, TViewModel>(),
                _queryable,
                _converter
            );
        }
    }
}