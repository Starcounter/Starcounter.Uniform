using System;

namespace Starcounter.Uniform
{
    public class UniDataTableColumn
    {
        public string Path { get; set; }
        public Type DataType { get; set; }
        public bool IsSortingAllowed { get; set; }
    }
}
