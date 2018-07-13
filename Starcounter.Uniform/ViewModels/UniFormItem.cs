using System.Collections.Generic;
using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FormItem;

namespace Starcounter.Uniform.ViewModels
{
    public class UniFormItem : Json
    {
        public Dictionary<string, MessageViewModel> MessageViewModels { get; set; }

        public void AddMessage(string property, string message, MessageType messageType)
        {
            MessageViewModels[property].AddMessage(message, this, messageType);
        }
    }
}
