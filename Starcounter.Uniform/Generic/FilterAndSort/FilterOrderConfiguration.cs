using System.Collections.Generic;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Combines filtering and sorting of tabular data
    /// </summary>
    public class FilterOrderConfiguration
    {
        public ICollection<Filter> Filters { get; set; }
        public ICollection<Order> Ordering { get; set; }
    }
}