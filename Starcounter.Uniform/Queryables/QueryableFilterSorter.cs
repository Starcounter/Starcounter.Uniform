using System;
using System.Linq;
using System.Linq.Expressions;
using Starcounter.Uniform.Generic.FilterAndSort;

namespace Starcounter.Uniform.Queryables
{
    /// <summary>
    /// Handles sorting and filtering queryable data. Can be overridden to support custom filter / order logic
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class QueryableFilterSorter<TData> : IQueryableFilterSorter<TData>
    {
        public IQueryable<TData> ApplyFilterAndOrder(IQueryable<TData> data, FilterOrderConfiguration configuration)
        {
            // todo add ApplyOrdering
            return ApplyFilters(data, configuration);
        }

        private IQueryable<TData> ApplyFilters(IQueryable<TData> data, FilterOrderConfiguration configuration)
        {
            foreach (var filter in configuration.Filters)
            {
                data = ApplyFilter(data, filter);
            }

            return data;
        }

        /// <summary>
        /// Override this method to support custom filters
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        protected virtual IQueryable<TData> ApplyFilter(IQueryable<TData> data, Filter filter)
        {
            var parameterExpression = Expression.Parameter(typeof(TData), "x");
            var propertyInfo = typeof(TData).GetProperty(filter.PropertyName);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException(
                    $"Could not apply filter: Type '{typeof(TData)}' has no property '{filter.PropertyName}'");
            }

            var propertyExpression = Expression.Property(parameterExpression, propertyInfo);
            var comparisonExpression = Expression.Equal(Expression.Constant(filter.Value), propertyExpression);
            var lambda = Expression.Lambda<Func<TData, bool>>(comparisonExpression, parameterExpression);

            return data.Where(lambda);
        }
    }
}