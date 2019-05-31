using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.Generic.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Uniform.ViewModels
{
    public sealed partial class UniDataTable : Json
    {
        public const string SortAscendingString = "asc";
        public const string SortDescendingString = "desc";

        public IFilteredDataProvider<Json> DataProvider { get; set; }

        private bool _isDisposed = false;

        public UniDataTable Init(IFilteredDataProvider<Json> dataProvider, IEnumerable<DataTableColumn> sourceColumns, int initialPageSize, int initialPageIndex)
        {
            CheckDisposed();
            this.DataProvider = dataProvider;

            Pagination.DataProvider = dataProvider;
            Pagination.LoadRows = LoadRows;
            Pagination.LoadRowsFromFirstPage = LoadRowsFromFirstPage;
            Pagination.GetTotalRows = () => (int)TotalRows;

            this.DataProvider.PaginationConfiguration = new PaginationConfiguration(initialPageSize, initialPageIndex);
            this.DataProvider.FilterOrderConfiguration = new FilterOrderConfiguration();

            PopulateColumns(sourceColumns);
            LoadRows();

            return this;
        }

        /// <summary>
        /// Loads rows for CurrentPageIndex from <see cref="PaginationConfiguration"/>. 
        /// When page is bigger than zero this method will add missing dummy pages to maintain sparse page indicies in Pages collection.
        /// </summary>
        public void LoadRows()
        {
            CheckDisposed();
            var page = this.DataProvider.PaginationConfiguration.CurrentPageIndex;
            if (page > 0)
            {
                // Add missing dummy pages to maintain sparse page indicies in Pages
                while (this.Pages.ElementAtOrDefault(page - 1) == null)
                {
                    this.Pages.Add();
                }
            }

            var newRowsData = new PagesViewModel();

            foreach (var currentPageRow in this.DataProvider.CurrentPageRows)
            {
                newRowsData.Rows.Add(currentPageRow);
            }

            if (this.Pages.ElementAtOrDefault(page) != null)
            {
                this.Pages[page] = newRowsData;
            }
            else
            {
                this.Pages.Add(newRowsData);
            }

            this.TotalRows = this.DataProvider.TotalRows;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            DataProvider?.Dispose();
            _isDisposed = true;
        }

        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void LoadRowsFromFirstPage()
        {
            this.DataProvider.PaginationConfiguration.CurrentPageIndex = 0;
            this.Pagination.CurrentPageIndex = 0;
            this.Pages.Clear();

            this.LoadRows();
        }

        private void PopulateColumns(IEnumerable<DataTableColumn> sourceColumns)
        {
            foreach (var sourceColumn in sourceColumns)
            {
                var column = this.Columns.Add();
                column.Data = sourceColumn;
                column.DataProvider = this.DataProvider;
                column.LoadRowsFromFirstPage = LoadRowsFromFirstPage;
            }
        }

        [UniDataTable_json.Pagination]
        public partial class PaginationViewModel : Json
        {
            public IFilteredDataProvider<Json> DataProvider { get; set; }
            public Action LoadRows { get; set; }
            public Action LoadRowsFromFirstPage { get; set; }
            public Func<int> GetTotalRows { get; set; }

            public int PageSize => DataProvider?.PaginationConfiguration.PageSize ?? 0;

            public int PagesCount => (GetTotalRows() + DataProvider?.PaginationConfiguration.PageSize - 1) /
                                     DataProvider?.PaginationConfiguration.PageSize ?? 0;

            public void Handle(Input.CurrentPageIndex action)
            {
                var newPageIndex = (int)action.Value;
                if (newPageIndex < 0)
                {
                    DataProvider.PaginationConfiguration.CurrentPageIndex = 0;
                }
                else
                {
                    DataProvider.PaginationConfiguration.CurrentPageIndex = newPageIndex > PagesCount ? PagesCount : newPageIndex;
                }

                LoadRows?.Invoke();
            }

            public void Handle(Input.PageSize action)
            {
                DataProvider.PaginationConfiguration.PageSize = (int)action.Value;
                LoadRowsFromFirstPage?.Invoke();
            }
        }

        [UniDataTable_json.Columns]
        public partial class ColumnsViewModel : Json, IBound<DataTableColumn>
        {
            private string _sortValue = null;

            public IFilteredDataProvider<Json> DataProvider { get; set; }
            public Action LoadRowsFromFirstPage { get; set; }

            /// <summary>
            /// The <see cref="string.Empty"/> value is replaced with NULL, 
            /// because empty string is also a "value" on the client side, which results into invalid HTML attribute:
            /// &lt;uni-data-table-sorter direction>
            /// while only
            /// &lt;uni-data-table-sorter direction="asc"> or &lt;uni-data-table-sorter direction="desc">
            /// are valid.
            /// </summary>
            public string Sort
            {
                get => _sortValue;
                set => _sortValue = string.IsNullOrWhiteSpace(value) ? null : value;
            }

            public void Handle(Input.Filter action)
            {
                var filter =
                    DataProvider.FilterOrderConfiguration.Filters.FirstOrDefault(x =>
                        x.PropertyName == this.PropertyName);
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(action.Value))
                    {
                        filter.Value = action.Value;
                    }
                    else
                    {
                        DataProvider.FilterOrderConfiguration.Filters.Remove(filter);
                    }
                }
                else
                {
                    DataProvider.FilterOrderConfiguration.Filters.Add(new Filter
                    {
                        PropertyName = this.PropertyName,
                        Value = action.Value
                    });
                }

                LoadRowsFromFirstPage?.Invoke();
            }

            public void Handle(Input.Sort action)
            {
                OrderDirection? direction = ParseOrderDirection(action.Value);
                Order order = this.DataProvider.FilterOrderConfiguration.Ordering.FirstOrDefault(x => x.PropertyName == this.PropertyName);

                if (!direction.HasValue)
                {
                    if (order != null)
                    {
                        this.DataProvider.FilterOrderConfiguration.Ordering.Remove(order);
                    }

                    return;
                }

                if (order == null)
                {
                    order = new Order()
                    {
                        PropertyName = this.PropertyName
                    };

                    this.DataProvider.FilterOrderConfiguration.Ordering.Add(order);
                }

                order.Direction = direction.Value;

                LoadRowsFromFirstPage?.Invoke();
            }

            private static OrderDirection? ParseOrderDirection(string orderString)
            {
                if (UniDataTable.SortAscendingString.Equals(orderString, StringComparison.InvariantCultureIgnoreCase))
                {
                    return OrderDirection.Ascending;
                }
                else if (UniDataTable.SortDescendingString.Equals(orderString, StringComparison.InvariantCultureIgnoreCase))
                {
                    return OrderDirection.Descending;
                }

                return null;
            }
        }

        [UniDataTable_json.Pages]
        public partial class PagesViewModel : Json
        {
        }
    }
}
