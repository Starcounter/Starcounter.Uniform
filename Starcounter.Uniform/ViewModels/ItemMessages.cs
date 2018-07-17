using Starcounter.Uniform.FormItem;
using Starcounter.Uniform.Generic.FormItem;
using System;
using System.Collections.Generic;

namespace Starcounter.Uniform.ViewModels
{
    public class ItemMessages : Json
    {
        internal Dictionary<string, MessageContainer> MessageContainers { get; set; }

        /// <summary>
        /// Gets message for given property.
        /// </summary>
        /// <param name="property">property name</param>
        /// <returns><see cref="FormItemMessage"/> object with message text and type.</returns>
        public FormItemMessage GetMessage(string property)
        {
            var messageContainer = GetMessageContainer(property);

            return messageContainer.GetMessage(this);
        }

        /// <summary>
        /// Sets message for given property.
        /// </summary>
        /// <param name="property">Property name</param>
        /// <param name="message">Message text</param>
        /// <param name="messageType">Message type as <see cref="MessageType"/></param>
        public void SetMessage(string property, string message, MessageType messageType)
        {
            var messageContainer = GetMessageContainer(property);

            messageContainer.AddMessage(message, this, messageType);
        }

        /// <summary>
        /// Clears message for given property.
        /// </summary>
        /// <param name="property">Property name</param>
        public void ClearMessage(string property)
        {
            var messageContainer = GetMessageContainer(property);

            messageContainer.ClearMessage(this);
        }

        /// <summary>
        /// Clears all messages for all declared properties.
        /// </summary>
        public void ClearAllMessages()
        {
            foreach (var messageContainersValue in MessageContainers.Values)
            {
                messageContainersValue.ClearMessage(this);
            }
        }

        /// <summary>
        /// Gets message container for given property, throws <see cref="ArgumentException"/> if failed.
        /// </summary>
        /// <param name="property">Property name</param>
        /// <returns><see cref="MessageContainer"/> object for given property name.</returns>
        private MessageContainer GetMessageContainer(string property)
        {
            MessageContainers.TryGetValue(property, out var messageContainer);
            if (messageContainer == null)
            {
                throw new ArgumentException("Property not found!", nameof(property));
            }

            return messageContainer;
        }
    }
}
