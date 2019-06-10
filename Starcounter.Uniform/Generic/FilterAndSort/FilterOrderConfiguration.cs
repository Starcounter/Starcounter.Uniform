using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Combines filtering and sorting of tabular data
    /// </summary>
    public class FilterOrderConfiguration
    {
        public FilterOrderConfiguration()
        {
        }

        public FilterOrderConfiguration(Order order, IEnumerable<Filter> filters)
        {
            Order = order;
            Filters = filters.ToList();
        }

        public List<Filter> Filters { get; } = new List<Filter>();

        public Order Order { get; set; }
    }
}