namespace Starcounter.Uniform.Builder
{
    /// <summary>
    /// Used to describe the structure of a data table
    /// </summary>
    public class DataTableColumn
    {
        /// <summary>
        /// Identifies a given column. Usually corresponds to a property name of a backing database object.
        /// </summary>
        public string PropertyName { get; set; }
        
        /// <summary>
        /// Default label for the column. Can be empty. Used only when html frontend doesn't specify custom template for the column.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Whether sorting by this column is allowed.
        /// </summary>
        public bool IsSortable { get; set; }

        /// <summary>
        /// Whether filtering by this column is allowed.
        /// </summary>
        public bool IsFilterable { get; set; }
    }
}