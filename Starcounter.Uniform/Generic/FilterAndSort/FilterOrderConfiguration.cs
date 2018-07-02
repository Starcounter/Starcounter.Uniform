using System.Collections.Generic;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Combines filtering and sorting of tabular data
    /// </summary>
    public class FilterOrderConfiguration
    {
        public IReadOnlyCollection<Filter> Filters { get; set; }
        public IReadOnlyCollection<Order> Ordering { get; set; }
    }
}