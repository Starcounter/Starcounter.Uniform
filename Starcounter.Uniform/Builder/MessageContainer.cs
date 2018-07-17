using Starcounter.Templates;
using Starcounter.Uniform.Generic.FormItem;

namespace Starcounter.Uniform.Builder
{
    public class MessageContainer
    {
        public MessageContainer(TObject container)
        {
            _invalid = container.Add<TString>("Invalid");
            _message = container.Add<TString>("Message");
        }

        private readonly TString _message;
        private readonly TString _invalid;

        public void AddMessage(string message, Json view, MessageType type)
        {
            view.Set(this._message, message);
            view.Set(this._invalid, ParseMessageType(type));
        }

        private string ParseMessageType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Invalid:
                    return "true";
                case MessageType.Valid:
                    return "false";
                default:
                    return string.Empty;
            }
        }
    }
}
