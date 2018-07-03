using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.Generic.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Uniform.ViewModels
{
    public partial class UniDataTable : Json
    {
        public IFilteredDataProvider<Json> FilteredDataProvider { get; set; }

        public UniDataTable Init(IFilteredDataProvider<Json> dataProvider, IEnumerable<DataTableColumn> sourceColumns, int initialPageSize, int initialPageIndex)
        {
            this.FilteredDataProvider = dataProvider;

            Pagination.DataProvider = dataProvider;
            Pagination.LoadRows = LoadRows;

            this.FilteredDataProvider.PaginationConfiguration = new PaginationConfiguration(initialPageSize, initialPageIndex);

            PopulateColumns(sourceColumns);
            LoadRows();

            return this;
        }

        private void PopulateColumns(IEnumerable<DataTableColumn> sourceColumns)
        {
            foreach (var sourceColumn in sourceColumns)
            {
                var column = this.Columnns.Add();
                column.Data = sourceColumn;
                column.DataProvider = this.FilteredDataProvider;
            }
        }

        private void LoadRows()
        {
            var page = this.FilteredDataProvider.PaginationConfiguration.CurrentPageIndex;
            if (page > 0)
            {
                // Add missing dummy pages to maintain sparse page indicies in Pages
                while (this.Pages.ElementAtOrDefault(page - 1) == null)
                {
                    this.Pages.Add();
                }
            }

            var newRowsData = new PagesViewModel();

            if (this.Pages.ElementAtOrDefault(page) == null)
            {
                this.Pages.Insert(page, newRowsData);
            }
            else
            {
                this.Pages[page] = newRowsData;
            }
        }

        [UniDataTable_json.Pagination]
        public partial class PaginationViewModel : Json
        {
            public IFilteredDataProvider<Json> DataProvider { get; set; }
            public Action LoadRows { get; set; }

            public int PagesCount => (DataProvider.TotalRows + DataProvider.PaginationConfiguration.PageSize - 1) / DataProvider.PaginationConfiguration.PageSize;
            public int PageSize => DataProvider.PaginationConfiguration.PageSize;

            void Handle(Input.CurrentPageIndex action)
            {
                DataProvider.PaginationConfiguration.CurrentPageIndex = (int)action.Value;
                LoadRows?.Invoke();
            }

            void Handle(Input.PageSize action)
            {
                DataProvider.PaginationConfiguration.PageSize = (int)action.Value;
                LoadRows?.Invoke(); // Reset all rows data and load only current page with new size?
            }
        }

        [UniDataTable_json.Pages]
        public partial class PagesViewModel : Json
        {
        }

        [UniDataTable_json.Columnns]
        public partial class ColumnsViewModel : Json, IBound<DataTableColumn>
        {
            public IFilteredDataProvider<Json> DataProvider { get; set; }

            void Handle(Input.Filter action)
            {
                var filter =
                    DataProvider.FilterOrderConfiguration.Filters.FirstOrDefault(x =>
                        x.PropertyName == this.PropertyName);
                if (filter != null)
                {
                    filter.Value = action.Value;
                }
                else
                {
                    DataProvider.FilterOrderConfiguration.Filters.Add(new Filter
                    {
                        PropertyName = this.PropertyName,
                        Value = action.Value
                    });
                }
            }

            private OrderDirection ParseOrderDirection(string orderString)
            {
                if (orderString == "asc")
                {
                    return OrderDirection.Ascending;
                }

                return OrderDirection.Descending;

            }

            void Handle(Input.Sort action)
            {
                var order =
                    DataProvider.FilterOrderConfiguration.Ordering.FirstOrDefault(x =>
                        x.PropertyName == this.PropertyName);
                if (order != null)
                {
                    if (!string.IsNullOrEmpty(action.Value))
                    {
                        order.Direction = ParseOrderDirection(action.Value);
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
            }
        }
    }
}
