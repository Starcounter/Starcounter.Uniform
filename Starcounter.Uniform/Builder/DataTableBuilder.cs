﻿using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Starcounter.Uniform.Builder
{
    /// <summary>
    /// Provides fluent API to create instances of <see cref="UniDataTable"/> view-model.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public class DataTableBuilder<TViewModel>
        where TViewModel : Json, new()
    {
        public const int DefaultPageSize = 50;

        private IFilteredDataProvider<TViewModel> _dataProvider;
        private IReadOnlyCollection<DataTableColumn> _columns = new DataTableColumn[0];
        private int _initialPageIndex;
        private int _initialPageSize = DefaultPageSize;
        private Order _initialOrder = null;

        /// <summary>
        /// Specify the data source for the table
        /// </summary>
        /// <typeparam name="TData">The type of original data, available as a queryable</typeparam>
        /// <param name="queryable">The original data to expose</param>
        /// <param name="configure">Configures the details of the data source</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataTableBuilder<TViewModel> WithDataSource<TData>(IQueryable<TData> queryable, Action<DataProviderBuilder<TData, TViewModel>> configure)
        {
            var builder = new DataProviderBuilder<TData, TViewModel>(queryable);
            configure(builder);
            _dataProvider = builder.Build();

            return this;
        }

        /// <summary>
        /// Specify the data source for the table
        /// </summary>
        /// <typeparam name="TData">The type of original data, available as a queryable</typeparam>
        /// <param name="queryable">The original data to expose</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataTableBuilder<TViewModel> WithDataSource<TData>(IQueryable<TData> queryable)
        {
            return WithDataSource(queryable, builder => { });
        }

        /// <summary>
        /// Specify the data source for the table. This method allows the developer to use custom implementation of <see cref="IFilteredDataProvider{TViewModel}"/>.
        /// It's usually sufficient to use other overloads that accept <see cref="IQueryable{TData}"/> directly
        /// </summary>
        /// <param name="dataProvider">The data source object</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataTableBuilder<TViewModel> WithDataSource(IFilteredDataProvider<TViewModel> dataProvider)
        {
            _dataProvider = dataProvider;

            return this;
        }

        /// <summary>
        /// Specify the column structure of the table. The app developer can skip calling this method, but it will require them to specify all columns
        /// in the html view.
        /// </summary>
        /// <param name="configure">Configures the details of column structure</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataTableBuilder<TViewModel> WithColumns(Action<DataColumnBuilder<TViewModel>> configure)
        {
            var columnBuilder = new DataColumnBuilder<TViewModel>();
            configure(columnBuilder);
            _columns = columnBuilder.Build();

            return this;
        }

        /// <summary>
        /// Specify the initial page index for the table. If this method is never called, the initial page index will be zero.
        /// </summary>
        /// <param name="initialPageIndex"></param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataTableBuilder<TViewModel> WithInitialPageIndex(int initialPageIndex)
        {
            if (initialPageIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialPageIndex), initialPageIndex, $"supplied value of {nameof(initialPageIndex)} = {initialPageIndex} is less than zero");
            }
            _initialPageIndex = initialPageIndex;
            return this;
        }

        /// <summary>
        /// Specify the initial page size for the table. If this method is never called, the initial page index will be equal to <see cref="DefaultPageSize"/>
        /// </summary>
        /// <param name="initialPageSize"></param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataTableBuilder<TViewModel> WithInitialPageSize(int initialPageSize)
        {
            _initialPageSize = initialPageSize;
            return this;
        }

        /// <summary>
        /// Specify the initial sort order for the table. If this method is never called the table will initially be unsorted.
        /// </summary>
        /// <param name="propertySelector">
        /// A function whose body is a <see cref="MemberExpression"/> referencing the property to sort by.
        /// </param>
        /// <param name="direction">The direction of the sort (ascending or descending).</param>
        /// <returns>The original builder object augmented with the specified sort order.</returns>
        public DataTableBuilder<TViewModel> WithInitialOrder<TProperty>(
            Expression<Func<TViewModel, TProperty>> propertySelector,
            OrderDirection direction = OrderDirection.Ascending
        )
        {
            var memberExpr = propertySelector.Body as MemberExpression;
            if (memberExpr == null)
            {
                throw new ArgumentException(
                    $"Expected a {nameof(MemberExpression)} as function body.",
                    nameof(propertySelector));
            }
            _initialOrder = new Order()
            {
                PropertyName = memberExpr.Member.Name,
                Direction = direction
            };
            return this;
        }

        public UniDataTable Build()
        {
            if (_dataProvider == null)
            {
                throw new InvalidOperationException($"DataSource has not been configured. Call one of {nameof(WithDataSource)} overloads before calling {nameof(Build)}");
            }

            return new UniDataTable().Init(_dataProvider, _columns, _initialPageSize, _initialPageIndex, _initialOrder);
        }
    }
}