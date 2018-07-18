# Starcounter.Uniform
Starcounter.Uniform is a helping library that provides server-side helpers for using and managing uniform components. 

It is available for downloading as [Starcounter.Uniform](https://www.nuget.org/packages/Starcounter.Uniform/) NuGet package.

## Requirements
Requires Starcounter 2.4.0.6535 or later and .NET Framework 4.6.1.

## How to use
First, in your Starcounter app project, install Starcounter.Uniform trough NuGet: `Install-Package Starcounter.Uniform` and add a reference to `Starcounter.Uniform.dll`.

This library provides helping method for a variety of uniform components and way to use it is slightly different for each one of them.

## Uni-data-table
`uni-data-table` is a custom element that is an adapter to `vaadin-grid`. It provides rich view of a data table which contain funcionalities like:
- Pagination or infinite scrolling.
- Sorting.
- Support for variety of column types.

More about `uni-data-table` component you can read in Uniform.css [readme for uni-data-table](https://github.com/Starcounter/uniform.css/tree/master/components/uni-data-table).

### Basic usage
To cover most of the cases we provide default implementation of all needed components to generate data table structure. Although `uni-data-table` implementation is fully customizable and user can reimplement all of its part to work as he want it to.

To create basic `uni-data-table` structure:

1. Provide container for data table structure in your json file:

```json
{
  "DataTable": {}
}

```

2. Create your data table row view-model, e.g.:
```json
{
  "FirstName": "",
  "LastName": "",
  "Email$": ""
}
```

3. In your handler where you return your view-model that contains data table, add data table inicialization with `DataTableBuilder`.
```cs
Handle.GET("/YourAppName/partial/datatable", () =>
            {
                return Db.Scope(() =>
                {
                    var dataTablePage = new DataTablePage();
                    dataTablePage.DataTable = new DataTableBuilder<DataTableRowViewModel>()
                        .WithDataSource(DbLinq.Objects<DataTableRowDataModel>())
                        .WithColumns(columns =>
                            columns
                                .AddColumn(b => b.FirstName,
                                    column => column.DisplayName("First Name").Sortable().Filterable())
                                .AddColumn(b => b.LastName, column => column.Sortable().DisplayName("Last Name"))
                                .AddColumn(b => b.Email,
                                    column => column.Filterable().Sortable())))
                        .Build();

                    return dataTablePage;
                });
            });
```

After those steps your container object in view view-model should be populated with our U`niFormItem` view-model that `uni-data-table` component will understand and work with.
You can find view-model structure used by `uni-data-table` on It's [readme page](https://github.com/Starcounter/uniform.css/tree/master/components/uni-data-table).

### DataTableBuilder
`DataTableBuilder` provides fluent API to easly create an instance of our UniDataTable view-model. To create It's instance you have to provide view-model for your rows data. Methods that `DataTableBuilder` provides are:

| Method Name | Arguments | Description | Returns |
| :--- | :--- | :--- | :--- |
| `WithDataSource` | `IQueryable<TData>` | Specifies the data source for the table with given `queryable` as the original data to expose. | The original `DataTableBuilder` with defined data source. |
| `WithDataSource` | `IQueryable<TData>`, `Action<DataProviderBuilder<TData, TViewModel>>` | Specifies the data source for the table with given `queryable` as the original data to expose and configuration for the details of the data source. | The original `DataTableBuilder` with defined data source. |
| `WithDataSource` | `IFilteredDataProvider<TViewModel>` | Specifies the data source for the table. This method allows the developer to use custom implementation of `IFilteredDataProvider`. | The original `DataTableBuilder` with defined data source. |
| `WithColumns` | `Action<DataColumnBuilder<TViewModel>>` | Speficies the column structure of the table. | The original `DataTableBuilder` with defined columns. |
| `WithInitialPageIndex` | `int` | Specify the initial page index for the table. If this method is never called, the initial page index will be zero. | The original `DataTableBuilder` with defined initial page index. |
| `WithInitialPageSize` | `int` | Specify the initial page size for the table. If this method is never called, the initial page index will be 50. | The original `DataTableBuilder` with defined initial page size. |
| `Build` | | Initializes the `UniDataTable` view-model with specified data source/provider, columns, initial page size and initial page index. | The `UniDataTable` view-model instance.

By using second variant of `WithDataSource` method, you can provide your own converter for creating row view-model instances, or/and your own way of applying filtering. Example implementation:
```c#
// In handle.GET
pageViewModel.DataTable = new DataTableBuilder<BookViewModel>()
      .WithDataSource(DbLinq.Objects<Book>(), data => data
          .WithConverter(CreateBookViewModel)
          .WithFilter(new BookFilter()))
      .WithColumns(columns =>
          columns
              .AddColumn(b => b.Position, column => column
                  .Sortable()
                  .Filterable()
                  .DisplayName("no. "))
              .AddColumn(b => b.Display, column => column
                  .Sortable()
                  .Filterable()
                  .DisplayName("Author&Title"))
      )
      .Build();                  

private BookViewModel CreateBookViewModel(Book book)
{
    var bookViewModel = new BookViewModel()
    {
        DeleteAction = DeleteBook 
    };
    ((Json) bookViewModel).Data = book;
    return bookViewModel;
}

private void DeleteBook(BookViewModel bookViewModel)
{
    bookViewModel.Data.Delete();
}

// BookFilter.cs
public class BookFilter : QueryableFilter<Book>
{
  protected override IQueryable<Book> ApplyFilter(IQueryable<Book> data, Filter filter)
  {
    if (filter.PropertyName == nameof(BookViewModel.Display))
    {
        return data.Where(book => book.Author == filter.Value || book.Title == filter.Value);
    }

    return base.ApplyFilter(data, filter);
  }
}
```
## History
For detailed changelog, check [Releases](https://github.com/Starcounter/Starcounter.Uniform/releases).

## License
MIT
