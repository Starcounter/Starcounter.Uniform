using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter.Templates;
using Starcounter.Uniform.Generic.FormItem;

namespace Starcounter.Uniform.Builder
{
    public class MessageViewModel
    {
        public MessageViewModel(TObject container)
        {
            _message = container.Add<TString>("Message");
            _invalid = container.Add<TString>("Invalid");
        }

        private readonly TString _message;
        private readonly TString _invalid;

        public void AddMessage(string message, Json view, MessageType type)
        {
            this._message.Setter(view, message);
            this._invalid.Setter(view, ParseMessageType(type));
        }

        private string ParseMessageType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Invalid:
                    return "invalid";
                case MessageType.Valid:
                    return "valid";
                default:
                    return string.Empty;
            }
        }
    }
}
