namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Describes a user intention to filter the data.
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Property by which the user wishes to filter the data.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// User-supplied value of the filter.
        /// </summary>
        public string Value { get; set; }
    }
}