using Starcounter.Uniform.Generic.Pagination;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Consumed by the data table control
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