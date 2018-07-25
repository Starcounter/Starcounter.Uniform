# Starcounter.Uniform
Starcounter.Uniform is a library that provides server-side helpers for using and managing uniform components. 

It is available for downloading as [Starcounter.Uniform](https://www.nuget.org/packages/Starcounter.Uniform/) NuGet package.

## Requirements
Requires Starcounter 2.4.0.6535 or later and .NET Framework 4.6.1.

## How to use
In your Starcounter app project, install Starcounter.Uniform with NuGet: `Install-Package Starcounter.Uniform`.

This library provides helping method for a variety of uniform components and way to use it is slightly different for each one of them.

## Uni-data-table
One of the components that `Starcounter.Uniform` covers is `uni-data-table`. It provides rich view of a data table which contain funcionalities like:
- Pagination or infinite scrolling.
- Sorting.
- Support for variety of column types.

More about `uni-data-table` component you can read in Uniform.css [readme for uni-data-table](https://github.com/Starcounter/uniform.css/tree/master/components/uni-data-table).

### Basic usage
To cover most of the cases we provide default implementation of all needed components to generate data table structure. Although `uni-data-table` implementation is fully customizable and users can reimplement all of its part to work as they want it to.

Let's say you want to add a data table with people information to your existing view-model `PeopleManagement`. To do that:

1. Add a property for the data table structure in your `PeopleManagement.json` file:

```json
{
  "DataTable": {}
}

```

2. Create your data table row view-model. Properties in this view-model will correspond to columns in the data table:
```json
{
  "FirstName": "",
  "LastName": "",
  "Email$": ""
}
```

3. In your `PeopleManagment.json.cs` add data table initialization with `DataTableBuilder`.
```cs
this.DataTable = new DataTablePage();
dataTablePage.DataTable = new DataTableBuilder<DataTableRowViewModel>()
    .WithDataSource(DbLinq.Objects<DataTableRowDataModel>())
    .WithColumns(columns =>
        columns
            .AddColumn(b => b.FirstName, column => column.DisplayName("First Name").Sortable().Filterable())
            .AddColumn(b => b.LastName, column => column.Sortable().DisplayName("Last Name"))
            .AddColumn(b => b.Email, column => column.Filterable().Sortable().DisplayName("Email")))
    .Build();
```
Note that you don't have to add a column for every property of the row view-model.

4. Add `<uni-data-table>` component to your view:
```html
<uni-data-table slot="uniformdocs/datatable-data-table"
                provider="{{model.DataTable}}" auto-pagination>
</uni-data-table>
```

After those steps your container object in view view-model should be populated with our `UniFormItem` view-model that `uni-data-table` component will understand and work with.
You can find view-model structure used by `uni-data-table` on its [readme page](https://github.com/Starcounter/uniform.css/blob/master/components/uni-data-table/README.md).

### DataTableBuilder
`DataTableBuilder` provides fluent API to easily create an instance of our UniDataTable view-model. To create its instance you have to provide view-model for your rows data. Methods that `DataTableBuilder` provides are:

| Method Name | Arguments | Description |
| :--- | :--- | :--- |
| `WithDataSource` | `IQueryable<TData>` | Specifies the data source for the table with given `queryable` as the original data to expose. |
| `WithDataSource` | `IQueryable<TData>`, `Action<DataProviderBuilder<TData, TViewModel>>` | Specifies the data source for the table with given `queryable` as the original data to expose and configuration for the details of the data source. |
| `WithDataSource` | `IFilteredDataProvider<TViewModel>` | Specifies the data source for the table. This method allows the developer to use custom implementation of `IFilteredDataProvider`. | The original `DataTableBuilder` with defined data source. |
| `WithColumns` | `Action<DataColumnBuilder<TViewModel>>` | Speficies the column structure of the table. |
| `WithInitialPageIndex` | `int` | Specify the initial page index for the table. If this method is never called, the initial page index will be zero. |
| `WithInitialPageSize` | `int` | Specify the initial page size for the table. If this method is never called, the initial page index will be 50. |
| `Build` | | Initializes the `UniDataTable` view-model with specified data source/provider, columns, initial page size and initial page index. |

### Computed columns
Sometimes, you want to add a column that has **no property** on its own or maybe its value is computed from multiple properties. You can do that. Just call `AddColumn` specifying the name manually:

```c#
.AddColumn("FullName", column => column.Sortable().Filterable().DisplayName("Full name"))
```
If you want to support filtering and/or sorting by this column, you have to extend the default `Filter`:

```c#
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
and then register it:
```c#
.WithDataSource(DbLinq.Objects<Book>(), data => data
          .WithFilter(new BookFilter()))
```

### Custom converter
Sometimes, you want to control the creation of row view-models. You can do that with `WithConverter` method.

First create your converter:
```c#
private BookViewModel CreateBookViewModel(Book book)
{
    var bookViewModel = new BookViewModel()
    {
        DeleteAction = DeleteBook 
    };
    ((Json) bookViewModel).Data = book;
    return bookViewModel;
}
```

and then register it:
```c#
.WithDataSource(DbLinq.Objects<Book>(), data => data
          .WithConverter(CreateBookViewModel)
```

### Example implementation

```c#
// In your page view model .cs file:
this.DataTable = new DataTableBuilder<BookViewModel>()
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

## Uni-form-item & uni-form-item-group
`uni-form-item` decorates an optional native `<label>`, any native or custom form control element, and an optional `<output>` message as form item. `uni-form-item-group` groups multiple `uni-form-item` elements into one form group with separate label and message.

You can read more about `uni-form-item` and `uni-form-item-group` components in Uniform.css [readme for uni-form-item](https://github.com/Starcounter/uniform.css/blob/master/components/uni-form-item/README.md) and [readme for uni-form-item-group](https://github.com/Starcounter/uniform.css/blob/master/components/uni-form-item-group/README.md).

### FormItemMessagesBuilder
`uni-form-item`/`uni-form-item-group` helper part works based on `FormItemMetadata` structure that is filled with proper view-model for given properties. Structure of this view-model looks like this:

```json
"FormItemMetadata": {
    "FieldName": {
        "Message": "",
        "Invalid": ""
    }    
}
```

To generate such structure, you have to use `FormItemMessagesBuilder()` which provides following methods:

| Method Name | Arguments | Description |
| :--- | :--- | :--- |
| `ForProperty` | `string` | Specifies the property to generate message structure for. |
| `ForProperties` | `IEnumerable<string>` | Specifies the properties to generate message structures for. |
| `Build` | | Initializes the `FormItemMetadata` for specified properties. |

### FormItemMetadata
After calling `FormItemMessagesBuilder` `Build()` method, you will end up with `FormItemMetadata` view-model with message structure for each previously described property. It also contains API for managing messages for each property:

| Method Name | Arguments | Description |
| :--- | :--- | :--- |
| `SetMessage` | `string`, `string`, `MessageType` | Sets the message of specified type and with specified text for the given property name. |
| `GetMessage` | `string` | Returns the `FormItemMessage` object, which contains text and type of a message for the given property name. |
| `ClearMessage` | `string` | Clears the message for the given property name. |
| `ClearAllMessages` | | Clears all the messages. |

### Example usage
To start using the helper in your `BookReservationPage` you have to:

1. Add a property for the `FormItemMetadata` structure in your `BookReservationPage.json` file:
```json
{
  "FormItemMetadata": {}
}
```

2. Set `InstanceType` for your newly added property to `FormItemMetadata`: 
```c#
static BookReservationPage()
{
    DefaultTemplate.FormItemMetadata.InstanceType = typeof(FormItemMetadata);
}
```
3. Initialize `FormItemMetadata` structure by specifing which properties should have proper metadata structure in `FormItemMessagesBuilder` (note that you can give it any property name, even one that is not present in your `.json` file.) and then calling its `Build()` method:
```c#
public void Init()
{
    this.FormItemMetadata = new FormItemMessagesBuilder().ForProperty(nameof(this.Title)).Build();
}
```

4. Start using it!:
```c#
void Handle(Input.Title action)
{
    if (action.Value == "A Song of Ice and Fire")
    {
        this.FormItemMetadata.SetMessage(
        nameof(this.Title),
        "This book is currently unavailable!",
        MessageType.Invalid);
    }
}
```

## History
For detailed changelog, check [Releases](https://github.com/Starcounter/Starcounter.Uniform/releases).

## License
MIT
