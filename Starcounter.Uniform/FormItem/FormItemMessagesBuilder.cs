using Starcounter.Templates;
using Starcounter.Uniform.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Uniform.FormItem
{
    public class FormItemMessagesBuilder
    {
        private List<string> _properties = new List<string>();

        /// <summary>
        /// Specify the property to generate message structure for.
        /// </summary>
        /// <param name="property">Property name</param>
        /// <returns>The original builder object</returns>
        public FormItemMessagesBuilder ForProperty(string property)
        {
            _properties.Add(property);

            return this;
        }

        /// <summary>
        /// Specify the properties to generate message structure for.
        /// </summary>
        /// <param name="properties">The list of properties names</param>
        /// <returns>The original builder object</returns>
        public FormItemMessagesBuilder ForProperties(List<string> properties)
        {
            _properties = _properties.Concat(properties).ToList();

            return this;
        }

        /// <summary>
        /// Builds the <see cref="ItemMessages"/> view-model object with message structures for given properties.
        /// </summary>
        /// <returns>The <see cref="ItemMessages"/> view-model object</returns>
        public ItemMessages Build()
        {
            var schema = new TObject();
            var messageContainers = _properties.ToDictionary(property => property, property => new MessageContainer(schema.Add<TObject>(property)));

            return new ItemMessages
            {
                Template = schema,
                MessageContainers = messageContainers
            };
        }
    }
}
