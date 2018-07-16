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
            var propertyName = "Name";

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

        [Test]
        public void AddColumnWithPropertySelectorAndConfigure()
        {
            var columns = _sut.AddColumn(row => row.Name, r => r.Sortable()).Build();

            columns.Should().ContainSingle(column => column.PropertyName == nameof(RowViewModel.Name)).Which.IsSortable.Should().Be(true);
        }

        [Test]
        public void AddColumnWithExplicitNameAndConfigure()
        {
            var propertyName = "Name";
            var displayName = "First name";
            var columns = _sut.AddColumn(propertyName, builder => builder.DisplayName(displayName)).Build();

            columns.Should().ContainSingle(column => column.PropertyName == propertyName).Which.DisplayName.Should()
                .Be(displayName);
        }
    }
}
