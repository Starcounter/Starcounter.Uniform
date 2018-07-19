namespace Starcounter.Uniform.FormItem
{
    /// <summary>
    /// Class that represents message object of a form item.
    /// </summary>
    public class FormItemMessage
    {
        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Can be "true", "false" or "" which represents invalid, valid and neutral message.
        /// </summary>
        public string Type { get; set; }
    }
}
