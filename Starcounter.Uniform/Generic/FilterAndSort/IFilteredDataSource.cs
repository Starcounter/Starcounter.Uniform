using System.Linq;
using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.Pagination;
using Starcounter.Uniform.Queryables;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Data source ready to be consumed by the data table control.
    /// Usually the app developer can use <see cref="FilteredPaginatedDataSource{TData,TViewModel}"/>, created using <see cref="DataTableBuilder{TViewModel}"/>.
    /// This interface can be implemented to expose data that is not <see cref="IQueryable"/>.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IFilteredDataSource<out TViewModel> : IPaginatedDataSource<TViewModel>
    {
        /// <summary>
        /// Implementers MUST ignore this property if it's null. This is to preserve the Liskov Substitution Principle
        /// </summary>
        FilterOrderConfiguration FilterOrderConfiguration { get; set; }
    }
}