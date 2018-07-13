using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter.Templates;
using Starcounter.Uniform.ViewModels;

namespace Starcounter.Uniform.Builder
{
    public class FormItemBuilder
    {
        private List<string> _properties = new List<string>();

        public FormItemBuilder ForProperty(string property)
        {
            _properties.Add(property);

            return this;
        }

        public FormItemBuilder ForProperties(List<string> properties)
        {
            _properties = _properties.Concat(properties).ToList();

            return this;
        }

        public UniFormItem Build()
        {
            var schema = new Json.JsonByExample.Schema();
            var messageViewModels = _properties.ToDictionary(property => property, property => new MessageViewModel(schema.Add<TObject>(property)));

            return new UniFormItem
            {
                Template = schema,
                MessageViewModels = messageViewModels
            };
        }
    }
}
