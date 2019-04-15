using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Starcounter.Uniform.Builder
{
    /// <summary>
    /// Provides fluent API to create instances of the column structure for data tables
    /// </summary>
    /// <typeparam name="TViewModel">The view-model for rows</typeparam>
    public class DataColumnBuilder<TViewModel>
    {
        private readonly List<DataTableColumn> _columns = new List<DataTableColumn>();

        /// <summary>
        /// Construct new <see cref="DataColumnBuilder{TViewModel}"/> instance
        /// </summary>
        /// <remarks>This class is not intended to be constructed by app developers directly. Rather, they should use it as part of <see cref="DataTableBuilder{TViewModel}"/></remarks>
        public DataColumnBuilder()
        {
        }

        /// <summary>
        /// Add a new column, named after a property in the view-model
        /// </summary>
        /// <typeparam name="TColumn">The type of the property</typeparam>
        /// <param name="propertySelector">Property access expression, used to derive the name of the new column</param>
        /// <param name="configure">Configuration for the column</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataColumnBuilder<TViewModel> AddColumn<TColumn>(
            Expression<Func<TViewModel, TColumn>> propertySelector,
            Action<ColumnBuilder> configure)
        {
            Type viewModelType = typeof(TViewModel);

            if (!(propertySelector.Body is MemberExpression member))
            {
                throw new ArgumentException($"Expression '{propertySelector}' refers to a method, not a property.");
            }

            if (!(member.Member is PropertyInfo propertyInfo))
            {
                throw new ArgumentException($"Expression '{propertySelector}' refers to a field, not a property.");
            }

            if (viewModelType != propertyInfo.ReflectedType && !viewModelType.IsSubclassOf(propertyInfo.ReflectedType) && propertyInfo?.DeclaringType?.DeclaringType != viewModelType)
            {
                throw new ArgumentException($"Expression '{propertySelector}' refers to a property that is not from type {viewModelType}.");
            }

            return AddColumn(propertyInfo.Name, configure);
        }

        /// <summary>
        /// Add a new column, named after a property in the view-model
        /// </summary>
        /// <typeparam name="TColumn">The type of the property</typeparam>
        /// <param name="propertySelector">Property access expression</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataColumnBuilder<TViewModel> AddColumn<TColumn>(
            Expression<Func<TViewModel, TColumn>> propertySelector)
        {
            return AddColumn(propertySelector, builder => { });
        }

        /// <summary>
        /// Add a new column, named explicitly
        /// </summary>
        /// <param name="propertyName">Name of the new column</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataColumnBuilder<TViewModel> AddColumn(
            string propertyName)
        {
            return AddColumn(propertyName, builder => { });
        }

        /// <summary>
        /// Add a new column, named explicitly
        /// </summary>
        /// <param name="propertyName">Name of the new column</param>
        /// <param name="configure">Configuration for the column</param>
        /// <returns>The original builder object</returns>
        /// <remarks>This method changes and returns the original builder object</remarks>
        public DataColumnBuilder<TViewModel> AddColumn(string propertyName, Action<ColumnBuilder> configure)
        {
            var column = new DataTableColumn()
            {
                PropertyName = propertyName,
                DisplayName = propertyName
            };
            configure(new ColumnBuilder(column));

            _columns.Add(column);

            return this;
        }

        /// <summary>
        /// Builds a column structure of a data table using configuration of this builder.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method is not intended to be used by app developers directly. Rather, they should use it as part of <see cref="DataTableBuilder{TViewModel}"/></remarks>
        public IReadOnlyCollection<DataTableColumn> Build()
        {
            return _columns;
        }

        /// <summary>
        /// Provides fluent API to specify a column of data table.
        /// </summary>
        public class ColumnBuilder
        {
            private readonly DataTableColumn _column;

            /// <summary>
            /// Construct new <see cref="DataTableColumn"/> instance
            /// </summary>
            /// <remarks>This class is not intended to be constructed by app developers directly. Rather, they should use it as part of <see cref="DataTableBuilder{TViewModel}"/></remarks>
            public ColumnBuilder(DataTableColumn column)
            {
                _column = column;
            }

            /// <summary>
            /// Specify <see cref="DataTableColumn.DisplayName"/> of the column
            /// </summary>
            /// <returns>The original builder object</returns>
            /// <remarks>This method changes and returns the original builder object</remarks>
            public ColumnBuilder DisplayName(string displayName)
            {
                _column.DisplayName = displayName;
                return this;
            }

            /// <summary>
            /// Specify whether sorting by this column will be allowed.
            /// </summary>
            /// <returns>The original builder object</returns>
            /// <remarks>This method changes and returns the original builder object</remarks>
            public ColumnBuilder Sortable(bool isSortable = true)
            {
                _column.IsSortable = isSortable;
                return this;
            }

            /// <summary>
            /// Specify whether filtering by this column will be allowed.
            /// </summary>
            /// <returns>The original builder object</returns>
            /// <remarks>This method changes and returns the original builder object</remarks>
            public ColumnBuilder Filterable(bool isFilterable = true)
            {
                _column.IsFilterable = isFilterable;
                return this;
            }
        }
    }
}