using Starcounter.Templates;
using Starcounter.Uniform.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Uniform.FormItem
{
    /// <summary>
    /// Provides fluent API to create instances of <see cref="FormItemMetadata"/>
    /// </summary>
    public class FormItemMessagesBuilder
    {
        private readonly List<string> _properties = new List<string>();

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
        public FormItemMessagesBuilder ForProperties(IEnumerable<string> properties)
        {
            _properties.AddRange(properties);

            return this;
        }

        /// <summary>
        /// Builds the <see cref="FormItemMetadata"/> view-model object with message structures for given properties.
        /// </summary>
        /// <returns>The <see cref="FormItemMetadata"/> view-model object</returns>
        public FormItemMetadata Build()
        {
            var formItemMetadata = new FormItemMetadata();
            formItemMetadata.Init(_properties);
            return formItemMetadata;
        }
    }
}
