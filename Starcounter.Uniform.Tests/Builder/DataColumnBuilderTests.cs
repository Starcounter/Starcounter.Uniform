using FluentAssertions;
using NUnit.Framework;
using Starcounter.Uniform.Builder;

namespace Starcounter.Uniform.Tests.Builder
{
    public class DataColumnBuilderTests
    {
        private DataColumnBuilder<RowViewModel> _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DataColumnBuilder<RowViewModel>();
        }

        [Test]
        public void AddColumnWithExplicitNameCreatesNamedColumn()
        {
            var propertyName = "name";

            var columns = _sut
                .AddColumn(propertyName)
                .Build();

            columns.Should().ContainSingle().Which.PropertyName.Should().Be(propertyName);
        }

        [Test]
        public void AddColumnFromPropertyCreatesNamedColumn()
        {
            var columns = _sut
                .AddColumn(row => row.Number)
                .Build();

            columns.Should().ContainSingle().Which.PropertyName.Should().Be(nameof(RowViewModel.Number));
        }
    }
}
