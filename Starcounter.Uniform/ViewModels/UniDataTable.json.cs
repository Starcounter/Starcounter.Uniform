using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter.Uniform.Interfaces;

namespace Starcounter.Uniform.ViewModels
{
    partial class UniDataTable : Json
    {
        public int PageSize => DataTableProvider.PageSize;
        public int Page => DataTableProvider.Page;
        public int RowsCount => DataTableProvider.CountRows();
        public IUniDataTableProvider DataTableProvider { get; protected set; }

        public void Init(IUniDataTableProvider dataTableProvider)
        {
            this.DataTableProvider = dataTableProvider;
        }

        void Handle(Input.Page action)
        {
            var page = (int)action.Value;
            this.DataTableProvider.Page = page;
            while (this.RowsData.ElementAtOrDefault(page - 1) == null)
            {
                this.RowsData.Add();
            }

            var newRowsData = new UniDataTableRowsData {Rows = this.DataTableProvider.SelectRows()};
            if (this.RowsData.ElementAtOrDefault(page) == null)
            {
                this.RowsData.Insert(page, newRowsData);
            }
            else
            {
                this.RowsData[page] = newRowsData;
            }
        }

        [UniDataTable_json.RowsData]
        partial class UniDataTableRowsData : Json
        {
            public IEnumerable<object> Rows { get; set; }

            static UniDataTableRowsData()
            {
                //DefaultTemplate.Rows - Provide schema for Rows
            }
        }

        /*
        /// <summary>
        /// Sorts and return given rows by specified column name and order
        /// </summary>
        /// <typeparam name="T">Type of row</typeparam>
        /// <param name="rows">Collection of rows to sort</param>
        /// <param name="columnToSortBy">Name of the column to sort by</param>
        /// <param name="order">Di</param>
        /// <returns></returns>
        public static IEnumerable<T> SortRows<T>(IEnumerable<T> rows, string columnToSortBy, SortOrder order)
        {
            if (order == SortOrder.Ascending)
            {
                return rows.OrderBy(x =>
                {
                    var propertyInfo = x.GetType().GetProperty(columnToSortBy);
                    return propertyInfo != null ? propertyInfo.GetValue(x) : x;
                });
            }
            else
            {
                return rows.OrderByDescending(x =>
                {
                    var propertyInfo = x.GetType().GetProperty(columnToSortBy);
                    return propertyInfo != null ? propertyInfo.GetValue(x) : x;
                });
            }
        }

        public static IEnumerable<T> FilterRows<T>(IEnumerable<T> rows, string columnToFilterBy, string filterString, bool lazyFiltering = false)
        {
            if (string.IsNullOrEmpty(filterString) || string.IsNullOrEmpty(columnToFilterBy) || rows.Any(x => x.GetType().GetProperty(columnToFilterBy)?.GetValue(x) == null))
            {
                return rows;
            }

            if (lazyFiltering)
            {
                return rows.Where(x => (string)x.GetType().GetProperty(columnToFilterBy)?.GetValue(x) == filterString);
            }
            else
            {
                return rows.Where(x => x.GetType().GetProperty(columnToFilterBy).GetValue(x).ToString().Contains(filterString));
            }
        }
        */
    }
}
