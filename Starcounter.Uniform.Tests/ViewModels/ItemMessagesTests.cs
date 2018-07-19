using FluentAssertions;
using NUnit.Framework;
using Starcounter.Uniform.FormItem;
using Starcounter.Uniform.Generic.FormItem;
using Starcounter.Uniform.ViewModels;
using System;
using System.Collections.Generic;

namespace Starcounter.Uniform.Tests.ViewModels
{
    public class ItemMessagesTests
    {
        private FormItemMetadata _sut;
        private List<string> _properties => new List<string>
        {
            "Property1",
            "Property2",
            "Property3"
        };

        private string _messageText => "Test message";

        [SetUp]
        public void SetUp()
        {
            _sut = new FormItemMessagesBuilder().ForProperties(_properties).Build();
        }

        [Test]
        public void SetMessageShouldSetMessageWithProperTextAndInvalidType()
        {
            _sut.SetMessage(_properties[0], _messageText, MessageType.Invalid);

            var message = _sut.GetMessage(_properties[0]);
            message.Text.Should().Be(_messageText);
            message.Type.Should().Be("true");
        }

        [Test]
        public void SetMessageNewTypeShouldSetProperMessageNewType()
        {
            _sut.SetMessage(_properties[0], _messageText, MessageType.Invalid);
            var message = _sut.GetMessage(_properties[0]);
            message.Type.Should().Be("true");

            _sut.SetMessage(_properties[0], _messageText, MessageType.Valid);
            message = _sut.GetMessage(_properties[0]);
            message.Type.Should().Be("false");
        }

        [Test]
        public void GetMessageForNotDeclaredPropertyShouldThrowArgumentException()
        {
            _sut.Invoking(vm => vm.GetMessage("NotExistingProperty")).Should().Throw<ArgumentException>()
                .WithMessage("NotExistingProperty was never declared. Make sure to call ForProperty(NotExistingProperty) builder method.");
        }

        [Test]
        public void SetMessageForNotDeclaredPropertyShouldThrowArgumentException()
        {
            _sut.Invoking(vm => vm.SetMessage("NotExistingProperty", _messageText, MessageType.Neutral)).Should().Throw<ArgumentException>()
                .WithMessage("NotExistingProperty was never declared. Make sure to call ForProperty(NotExistingProperty) builder method.");
        }

        [Test]
        public void ClearMessageShouldRemoveTextAndTypeOfAMessage()
        {
            _sut.SetMessage(_properties[0], _messageText, MessageType.Invalid);

            _sut.ClearMessage(_properties[0]);

            var message = _sut.GetMessage(_properties[0]);
            message.Text.Should().BeEmpty();
            message.Type.Should().BeEmpty();
        }

        [Test]
        public void ClearAllMessagesShouldRemoveTextAndTypeOfAllMessages()
        {
            _sut.SetMessage(_properties[0], _messageText, MessageType.Invalid);
            _sut.SetMessage(_properties[1], _messageText, MessageType.Valid);

            _sut.ClearAllMessages();

            var message = _sut.GetMessage(_properties[0]);
            message.Text.Should().BeEmpty();
            message.Type.Should().BeEmpty();
            message = _sut.GetMessage(_properties[1]);
            message.Text.Should().BeEmpty();
            message.Type.Should().BeEmpty();
        }

        [Test]
        public void SetMultiplePropertiesMessagesShouldSetAllProperMessages()
        {
            _sut.SetMessage(_properties[0], "Test message 0", MessageType.Invalid);
            _sut.SetMessage(_properties[1], "Test message 1", MessageType.Valid);
            _sut.SetMessage(_properties[2], "Test message 2", MessageType.Neutral);

            var message = _sut.GetMessage(_properties[0]);
            message.Text.Should().Be("Test message 0");
            message.Type.Should().Be("true");
            message = _sut.GetMessage(_properties[1]);
            message.Text.Should().Be("Test message 1");
            message.Type.Should().Be("false");
            message = _sut.GetMessage(_properties[2]);
            message.Text.Should().Be("Test message 2");
            message.Type.Should().BeEmpty();
        }
    }
}
