using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.Generic.Pagination;
using System;

namespace Starcounter.Uniform.ViewModels
{
    public partial class UniDataTable : Json
    {
        public IFilteredDataSource<Json> FilteredDataSource { get; protected set; }

        public void Init(IFilteredDataSource<Json> dataSource, Arr<ColumnnsElementJson> sourceColumns) // Will be changed to specific class for Column
        {
            //var ints = Enumerable.Empty<string>(); // OUT
            //IEnumerable<object> list = ints;

            //Action<object> action = o => { }; // IN
            //Action<string> a = action;

            this.FilteredDataSource = dataSource;
            Pagination.DataSource = dataSource;
            Pagination.ReloadRows = ReloadRows;
            this.FilteredDataSource.PaginationConfiguration = new PaginationConfiguration(100);

            this.Columnns = sourceColumns; // Populate columns
        }

        private void ReloadRows()
        {
            this.Pages = FilteredDataSource.CurrentPageRows // Find proper page to put proper rows.
        }


        [UniDataTable_json.Pagination]
        public partial class PaginationViewModel : Json
        {
            //TODO: PagesCount? When change it? Which it should refer to?
            public int PagesCount => (DataSource.TotalRows + DataSource.PaginationConfiguration.PageSize - 1) / DataSource.PaginationConfiguration.PageSize; // Rounded up
            public IPaginatedDataSource<Json> DataSource { get; set; }
            public Action ReloadRows { get; set; }

            void Handle(Input.CurrentPageIndex action)
            {
                DataSource.PaginationConfiguration.CurrentPageIndex = (int)action.Value;
                ReloadRows?.Invoke();
            }

            void Handle(Input.PageSize action)
            {
                DataSource.PaginationConfiguration.PageSize = (int)action.Value;
                ReloadRows?.Invoke();
            }
        }
    }
}
