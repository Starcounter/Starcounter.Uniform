namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Describes a user intention to order the data.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Property by which the user wishes to order the data.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// The direction of ordering.
        /// </summary>
        public OrderDirection Direction { get; set; }
    }
}