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
        public IFilteredDataProvider<Json> FilteredDataSource { get; set; }

        public void Init(IFilteredDataProvider<Json> dataSource, IEnumerable<DataTableColumn> sourceColumns, int initialPageSize, int initialPageIndex)
        {
            this.FilteredDataSource = dataSource;

            Pagination.DataSource = dataSource;
            Pagination.LoadRows = LoadRows;

            this.FilteredDataSource.PaginationConfiguration = new PaginationConfiguration(initialPageSize, initialPageIndex);

            PopulateColumns(sourceColumns);
            LoadRows();
        }

        private void PopulateColumns(IEnumerable<DataTableColumn> sourceColumns)
        {
            foreach (var sourceColumn in sourceColumns)
            {
                var column = this.Columnns.Add();
                column.Data = sourceColumn;
            }
        }

        private void LoadRows()
        {
            var page = this.FilteredDataSource.PaginationConfiguration.CurrentPageIndex;
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
            public IFilteredDataProvider<Json> DataSource { get; set; }
            public Action LoadRows { get; set; }

            public int PagesCount => (DataSource.TotalRows + DataSource.PaginationConfiguration.PageSize - 1) / DataSource.PaginationConfiguration.PageSize;
            public int PageSize => DataSource.PaginationConfiguration.PageSize;

            void Handle(Input.CurrentPageIndex action)
            {
                DataSource.PaginationConfiguration.CurrentPageIndex = (int)action.Value;
                LoadRows?.Invoke();
            }

            void Handle(Input.PageSize action)
            {
                DataSource.PaginationConfiguration.PageSize = (int)action.Value;
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
        }
    }
}
