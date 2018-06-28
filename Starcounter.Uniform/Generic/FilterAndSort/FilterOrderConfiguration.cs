using System.Collections.Generic;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    public class FilterOrderConfiguration
    {
        public IReadOnlyCollection<Filter> Filters { get; set; }
        public IReadOnlyCollection<Order> Ordering { get; set; }
    }
}