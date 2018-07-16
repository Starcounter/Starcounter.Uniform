using Starcounter.Uniform.Generic.FilterAndSort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Starcounter.Uniform.Queryables
{
    /// <summary>
    /// Handles sorting and filtering of queryable data. Can be overridden to support custom filter / order logic
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class QueryableFilter<TData> : IQueryableFilter<TData>
    {
        public IQueryable<TData> Apply(IQueryable<TData> data, FilterOrderConfiguration configuration)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            data = ApplyFilters(data, configuration.Filters);
            data = ApplyOrdering(data, configuration.Ordering);
            return data;
        }

        private IQueryable<TData> ApplyFilters(IQueryable<TData> data, ICollection<Filter> filters)
        {
            foreach (var filter in filters)
            {
                data = ApplyFilter(data, filter);
            }

            return data;
        }

        private IQueryable<TData> ApplyOrdering(IQueryable<TData> data, ICollection<Order> ordering)
        {
            foreach (var order in ordering)
            {
                data = ApplyOrder(data, order);
            }

            return data;
        }

        /// <summary>
        /// Override this method to support custom filters
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filter"></param>
        /// <returns>A new queryable, representing filtered data</returns>
        protected virtual IQueryable<TData> ApplyFilter(IQueryable<TData> data, Filter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            var propertyInfo = PropertyInfoFromName(filter.PropertyName);

            var parameterExpression = Expression.Parameter(typeof(TData), "x");
            var propertyExpression = Expression.Property(parameterExpression, propertyInfo);

            var filterMethodExp = GetFilterMethodCallExpression(propertyInfo.PropertyType, propertyExpression, filter.Value);
            if (filterMethodExp != null)
            {
                var lambda = Expression.Lambda<Func<TData, bool>>(filterMethodExp, parameterExpression);

                return data.Where(lambda);
            }
            else
            {
                return Enumerable.Empty<TData>().AsQueryable();
            }
        }

        /// <summary>
        /// Override this method to support custom ordering
        /// </summary>
        /// <param name="data"></param>
        /// <param name="order"></param>
        /// <returns>A new queryable, representing filtered data</returns>
        protected virtual IQueryable<TData> ApplyOrder(IQueryable<TData> data, Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (order.Direction != OrderDirection.Ascending && order.Direction != OrderDirection.Descending)
            {
                throw new ArgumentOutOfRangeException(nameof(order), $"Invalid value for {nameof(order)}: '{order}'");
            }

            var propertyInfo = PropertyInfoFromName(order.PropertyName);

            var parameterExpression = Expression.Parameter(typeof(TData), "x");
            var propertyExpression = Expression.Property(parameterExpression, propertyInfo);
            // underlying type is Expression.Lambda<Func<TData, {propertyInfo.PropertyType}>>
            var lambda = Expression.Lambda(propertyExpression, parameterExpression);

            // it's possible to cache that into a pair of delegates (for each direction) later,
            // if performance demands it
            return (IQueryable<TData>)GetGenericOrderByDirection(propertyInfo.PropertyType,
                    order.Direction)
                .Invoke(null, // invoking static method
                    new object[]
                    {
                        data,
                        lambda
                    });
        }

        private static MethodCallExpression GetFilterMethodCallExpression(Type keyType, MemberExpression propertyExpression, string filterValue)
        {
            if (keyType == typeof(string))
            {
                var method = typeof(string).GetMethod(nameof(string.Contains));
                return Expression.Call(propertyExpression, method, Expression.Constant(filterValue));
            }
            else
            {
                var method = typeof(int).GetMethod(nameof(int.Equals), new[] { typeof(int) });
                var isFilterValueANumber = int.TryParse(filterValue, out var intFilterValue);

                return isFilterValueANumber ? Expression.Call(propertyExpression, method, Expression.Constant(intFilterValue)) : null;
            }
        }

        private static MethodInfo GetGenericOrderByDirection(Type keyType, OrderDirection direction)
        {
            var methodName = direction == OrderDirection.Ascending
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending);

            return typeof(Queryable)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.Name == methodName)
                // select overload which accepts only queryable and keySelector
                .Single(method => method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TData), keyType);
        }

        private static PropertyInfo PropertyInfoFromName(string propertyName)
        {
            var propertyInfo = typeof(TData).GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new InvalidOperationException(
                    $"Could not apply filter: Type '{typeof(TData)}' has no property '{propertyName}'");
            }

            return propertyInfo;
        }
    }
}