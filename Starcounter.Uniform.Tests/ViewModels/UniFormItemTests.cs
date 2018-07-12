using NUnit.Framework;
using Starcounter.Uniform.Generic.FormItem;
using Starcounter.Uniform.ViewModels;

namespace Starcounter.Uniform.Tests.ViewModels
{
    public class UniFormItemTests
    {
        private UniFormItem _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new UniFormItem();
        }

        [Test]
        public void Test()
        {
            _sut.AddMessage("Test", "TestMessage", MessageType.Invalid);
        }
    }
}
