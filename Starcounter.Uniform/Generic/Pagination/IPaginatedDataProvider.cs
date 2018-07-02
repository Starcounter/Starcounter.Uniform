using System.Collections.Generic;

namespace Starcounter.Uniform.Generic.Pagination
{
    /// <summary>
    /// Consumed by pagination controls
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IPaginatedDataProvider<out TViewModel>
    {
        /// <summary>
        /// Set it, or change its properties to change the value of <see cref="CurrentPageRows"/>
        /// </summary>
        PaginationConfiguration PaginationConfiguration { get; set; }

        /// <summary>
        /// Reflects the current page of rows. Returned collection should have size specified in <see cref="PaginationConfiguration"/>.
        /// Every call to this getter should return latest data, reflecting any changes in <see cref="PaginationConfiguration"/>.
        /// </summary>
        IReadOnlyCollection<TViewModel> CurrentPageRows { get; }

        /// <summary>
        /// The total number of rows of underlying data source.
        /// </summary>
        int TotalRows { get; }
    }
}