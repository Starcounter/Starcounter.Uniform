using System.Collections.Generic;

namespace Starcounter.Uniform.Generic.Pagination
{
    /// <summary>
    /// Consumed by pagination controls
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IPaginatedDataProvider<out TViewModel>
    {
        PaginationConfiguration PaginationConfiguration { get; set; }
        IReadOnlyCollection<TViewModel> CurrentPageRows { get; }
        int TotalRows { get; }
    }
}