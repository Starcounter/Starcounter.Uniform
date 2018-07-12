using Starcounter.Templates;
using Starcounter.Uniform.Generic.FormItem;

namespace Starcounter.Uniform.ViewModels
{
    public partial class UniFormItem : Json
    {
        public void AddMessage(string propertyName, string message, MessageType messageType)
        {
            var newSchema = new JsonByExample.Schema();
            var arraySchema = newSchema.Add<TObjArr>("Title");
            var messageEntrySchema = new TObject();
            messageEntrySchema.Add<TString>("Text");
            messageEntrySchema.Add<TString>("Invalid");
            arraySchema.ElementType = messageEntrySchema;

            this.Template = newSchema;
            //ItemMessages = new ItemMessagesViewModel();
            // ItemMessages.AddMessage(nameof(RowsCount), "Too many rows", "some enum");
        }
    }
}
