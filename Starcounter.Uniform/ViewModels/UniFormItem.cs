using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FormItem;
using System;
using System.Collections.Generic;

namespace Starcounter.Uniform.ViewModels
{
    public class UniFormItem : Json
    {
        public Dictionary<string, MessageContainer> MessageContainers { get; set; }

        public void AddMessage(string property, string message, MessageType messageType)
        {
            MessageContainers.TryGetValue(property, out var messageViewModel);
            if (messageViewModel == null)
            {
                throw new ArgumentException("Property not found!");
            }

            messageViewModel.AddMessage(message, this, messageType);
        }
    }
}
