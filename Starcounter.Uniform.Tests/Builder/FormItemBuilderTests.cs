using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FormItem;
using Starcounter.Uniform.ViewModels;

namespace Starcounter.Uniform.Tests.Builder
{
    public class FormItemBuilderTests
    {
        private FormItemBuilder _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new FormItemBuilder();
        }

        [Test]
        public void Test()
        {
            var mod = _sut.ForProperty("Test").Build();
            mod.AddMessage("Test", "Test message", MessageType.Invalid);
        }
    }
}
