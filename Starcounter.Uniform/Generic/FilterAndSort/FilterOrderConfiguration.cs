using System.Collections.Generic;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Combines filtering and sorting of tabular data
    /// </summary>
    public class FilterOrderConfiguration
    {
        public List<Filter> Filters { get; } = new List<Filter>();
        public List<Order> Ordering { get; } = new List<Order>();

    }
}