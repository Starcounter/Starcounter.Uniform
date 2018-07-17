using Starcounter.Templates;
using Starcounter.Uniform.Generic.FormItem;

namespace Starcounter.Uniform.FormItem
{
    public class MessageContainer
    {
        public MessageContainer(TObject container)
        {
            _invalid = container.Add<TString>("Invalid");
            _message = container.Add<TString>("Message");
            _container = container;
        }

        private readonly TObject _container;
        private readonly TString _message;
        private readonly TString _invalid;

        public void AddMessage(string message, Json view, MessageType type)
        {
            view.Get(_container).Set(this._message, message);
            view.Get(_container).Set(this._invalid, ParseMessageType(type));
        }

        public FormItemMessage GetMessage(Json view)
        {
            return new FormItemMessage
            {
                Text = view.Get(_container).Get(this._message),
                Type = view.Get(_container).Get(this._invalid)
            };
        }

        public void ClearMessage(Json view)
        {
            view.Get(_container).Set(this._message, string.Empty);
            view.Get(_container).Set(this._invalid, string.Empty);
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
