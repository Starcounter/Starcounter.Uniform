using FluentAssertions;
using NUnit.Framework;
using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.Queryables;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Uniform.Tests.Queryables
{
    public class QueryableFilterTests
    {
        private QueryableFilter<RowDataModel> _sut;
        private IQueryable<RowDataModel> _rowDataModels = new List<RowDataModel>
        {
            new RowDataModel {Name = "Tom", Number = 0, Flag = true},
            new RowDataModel {Name = "Ann", Number = 2, Flag = false},
            new RowDataModel {Name = "Clark", Number = 1},
            new RowDataModel {Name = "Amanda", Number = 3},
            new RowDataModel {Name = "Abi", Number = 4}
        }.AsQueryable();

        [SetUp]
        public void SetUp()
        {
            _sut = new QueryableFilter<RowDataModel>();
        }

        private IQueryable<RowDataModel> ApplyFilteringOrdering(IEnumerable<Filter> filters = null, IEnumerable<Order> orderings = null)
        {
            var filterOrderConfiguration = new FilterOrderConfiguration();

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    filterOrderConfiguration.Filters.Add(filter);
                }
            }

            if (orderings != null)
            {
                foreach (var ordering in orderings)
                {
                    filterOrderConfiguration.Ordering.Add(ordering);
                }
            }

            var returnedData = _sut.Apply(_rowDataModels, filterOrderConfiguration);

            return returnedData;
        }

        [Test]
        public void ApplyExactFilterShouldReturnRowDataWithTheSameNameAsSpecified()
        {
            var nameToFilter = "Ann";
            var filter = new Filter { PropertyName = "Name", Value = nameToFilter };

            var returnedData = ApplyFilteringOrdering(new[] { filter });
            returnedData.Should().ContainSingle().Which.Name.Should().Be(nameToFilter);
        }

        [Test]
        public void ApplyPartialFilterShouldReturnProperRowDatas()
        {
            var filter = new Filter { PropertyName = "Name", Value = "A" };
            var returnedData = ApplyFilteringOrdering(new[] { filter });

            returnedData.Should().NotContain(x => x.Name == "Tom");
        }

        [Test]
        public void ApplyStringFilterShouldReturnProperRowDatas()
        {
            var aFilter = new Filter { PropertyName = "Name", Value = "Clark" };
            var returnedData = ApplyFilteringOrdering(new[] { aFilter });

            returnedData.Should().ContainSingle().Which.Name.Should().Be("Clark");
        }

        [Test]
        public void ApplyNumberFilterShouldReturnProperRowDatas()
        {
            var numberFilter = new Filter { PropertyName = "Number", Value = "3" };
            var returnedData = ApplyFilteringOrdering(new[] { numberFilter });

            returnedData.Should().ContainSingle().Which.Name.Should().Be("Amanda");
        }

        [Test]
        public void ApplyIncorrectNumberFilterShouldReturnProperRowDatas()
        {
            var numberFilter = new Filter { PropertyName = "Number", Value = "3.5" };
            var returnedData = ApplyFilteringOrdering(new[] { numberFilter });

            returnedData.Should().BeEmpty();
        }

        [Test]
        public void ApplyBooleanFilterShouldReturnProperRowDatas()
        {
            var boolFilter = new Filter { PropertyName = "Flag", Value = "true" };
            var returnedData = ApplyFilteringOrdering(new[] { boolFilter });

            returnedData.Should().ContainSingle().Which.Name.Should().Be("Tom");
        }

        [Test]
        public void ApplyIncorrectTypeFilterShouldReturnEmptyRowDatas()
        {
            var flagFilter = new Filter { PropertyName = "Flag", Value = "something" };
            var returnedData = ApplyFilteringOrdering(new[] { flagFilter });

            returnedData.Should().BeEmpty();
        }

        [Test]
        public void ApplyingMultipleFiltersShouldReturnProperRowsDatas()
        {
            var aFilter = new Filter { PropertyName = "Name", Value = "A" };
            var numberFilter = new Filter { PropertyName = "Number", Value = "4" };
            var returnedData = ApplyFilteringOrdering(new[] { aFilter, numberFilter });

            returnedData.Should().ContainSingle().Which.Name.Should().Be("Abi");
        }

        [Test]
        public void ApplyingOrderingShouldReturnProperRowsDatasOrdered()
        {
            var order = new Order { PropertyName = nameof(RowDataModel.Name), Direction = OrderDirection.Ascending };
            var returnedData = ApplyFilteringOrdering(null, new[] { order });

            returnedData.Select(model => model.Name).Should().BeInAscendingOrder();
        }

        [Test]
        public void ApplyingIntOrderingShouldReturnProperRowsDatasOrdered()
        {
            var order = new Order { PropertyName = nameof(RowDataModel.Number), Direction = OrderDirection.Ascending };
            var returnedData = ApplyFilteringOrdering(null, new[] { order });

            returnedData.Select(model => model.Number).Should().BeInAscendingOrder();
        }

        [Test]
        public void ApplyingMultipleOrderingShouldReturnProperRowsDatasOrdered()
        {
            _rowDataModels = new List<RowDataModel>
            {
                new RowDataModel {Name = "Tom", Number = 0},
                new RowDataModel {Name = "Ann", Number = 2},
                new RowDataModel {Name = "Clark", Number = 1},
                new RowDataModel {Name = "Amanda", Number = 2}
            }.AsQueryable();

            var expectedOrderedData = new List<RowDataModel>
            {
                new RowDataModel {Name = "Tom", Number = 0},
                new RowDataModel {Name = "Clark", Number = 1},
                new RowDataModel {Name = "Amanda", Number = 2},
                new RowDataModel {Name = "Ann", Number = 2}
            }.AsQueryable();

            var nameOrder = new Order { PropertyName = "Name", Direction = OrderDirection.Ascending };
            var numberOrder = new Order { PropertyName = "Number", Direction = OrderDirection.Ascending };
            var returnedData = ApplyFilteringOrdering(null, new[] { nameOrder, numberOrder });

            returnedData.Should().BeEquivalentTo(expectedOrderedData, options => options.WithStrictOrdering());
        }

        [Test]
        public void ApplyingMixOfFilterAndOrderShouldReturnProperRowsDatasFilteredAndOrdered()
        {
            var expectedOrderedData = new List<RowDataModel>
            {
                new RowDataModel {Name = "Abi", Number = 4},
                new RowDataModel {Name = "Amanda", Number = 3},
                new RowDataModel {Name = "Ann", Number = 2}
            }.AsQueryable();
            var filter = new Filter { PropertyName = "Name", Value = "A" };
            var order = new Order { PropertyName = "Name", Direction = OrderDirection.Ascending };

            var returnedData = ApplyFilteringOrdering(new[] { filter }, new[] { order });

            returnedData.Should().BeEquivalentTo(expectedOrderedData, options => options.WithStrictOrdering());
        }
    }
}
