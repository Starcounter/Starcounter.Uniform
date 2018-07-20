using Starcounter.Uniform.Generic.FormItem;

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
        /// Represents type of the message. Message type can be either invalid ("true"), valid ("false") or neutral ("").
        /// </summary>
        public MessageType Type { get; set; }
    }
}
