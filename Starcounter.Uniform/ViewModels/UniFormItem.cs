using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FormItem;
using System;
using System.Collections.Generic;

namespace Starcounter.Uniform.ViewModels
{
    public class UniFormItem : Json
    {
        public Dictionary<string, MessageViewModel> MessageViewModels { get; set; }

        public void AddSo(string property, MessageType type)
        {
            MessageViewModels[property].AddSo(type, this);
        }

        public void AddMessage(string property, string message, MessageType messageType)
        {
            MessageViewModels.TryGetValue(property, out var messageViewModel);
            if (messageViewModel == null)
            {
                throw new ArgumentException("Property not found!");
            }

            messageViewModel.AddMessage(message, this, messageType);
        }
    }
}
