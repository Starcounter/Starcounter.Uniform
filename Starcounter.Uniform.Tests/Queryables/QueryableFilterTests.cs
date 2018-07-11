using System;
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

        [SetUp]
        public void SetUp()
        {
            _sut = new QueryableFilter<RowDataModel>();
        }

        [Test]
        public void ApplyExactFilterShouldReturnRowDataWithTheSameNameAsSpecified()
        {
            var nameToFilter = "Ann";
            var data = new List<RowDataModel>
            {
                new RowDataModel {Name = nameToFilter, Number = 0},
                new RowDataModel {Name = "Amanda", Number = 1},
                new RowDataModel {Name = "Tom", Number = 2}
            }.AsQueryable();
            var filter = new Filter { PropertyName = "Name", Value = nameToFilter };
            var filterOrderConfiguration = new FilterOrderConfiguration();
            filterOrderConfiguration.Filters.Add(filter);

            var returnedData = _sut.Apply(data, filterOrderConfiguration);

            returnedData.Should().ContainSingle().Which.Name.Should().Be(nameToFilter);
        }

        [Test]
        public void ApplyPartialFilterShouldReturnProperRowDatas()
        {
            var data = new List<RowDataModel>
            {
                new RowDataModel {Name = "Ann", Number = 0},
                new RowDataModel {Name = "Amanda", Number = 1},
                new RowDataModel {Name = "Tom", Number = 2}
            }.AsQueryable();
            var filter = new Filter { PropertyName = "Name", Value = "A" };
            var filterOrderConfiguration = new FilterOrderConfiguration();
            filterOrderConfiguration.Filters.Add(filter);

            var returnedData = _sut.Apply(data, filterOrderConfiguration);

            returnedData.Should().HaveCount(2);
            returnedData.Should().NotContain(x => x.Name == "Tom");
        }

        [Test]
        public void ApplyingMultipleFiltersShouldReturnProperRowsDatas()
        {
            var data = new List<RowDataModel>
            {
                new RowDataModel {Name = "Ann", Number = 0},
                new RowDataModel {Name = "Amanda", Number = 1},
                new RowDataModel {Name = "Tom", Number = 2},
                new RowDataModel {Name = "Clark", Number = 3},
            }.AsQueryable();
            var aFilter = new Filter { PropertyName = "Name", Value = "A" };
            var annFilter = new Filter { PropertyName = "Name", Value = "Ann" };
            var filterOrderConfiguration = new FilterOrderConfiguration();
            filterOrderConfiguration.Filters.Add(aFilter);
            filterOrderConfiguration.Filters.Add(annFilter);

            var returnedData = _sut.Apply(data, filterOrderConfiguration);

            returnedData.Should().ContainSingle().Which.Name.Should().Be("Ann");
        }

        [Test]
        public void ApplyingOrderingShouldReturnProperRowsDatasOrdered()
        {
            var data = new List<RowDataModel>
            {
                new RowDataModel {Name = "Tom", Number = 2},
                new RowDataModel {Name = "Ann", Number = 0},
                new RowDataModel {Name = "Clark", Number = 2},
                new RowDataModel {Name = "Amanda", Number = 1}
            }.AsQueryable();

            var filterOrderConfiguration = new FilterOrderConfiguration()
            {
                Ordering = {new Order {PropertyName = nameof(RowDataModel.Name), Direction = OrderDirection.Ascending}}
            };

            var returnedData = _sut.Apply(data, filterOrderConfiguration);

            returnedData.Select(model => model.Name).Should().BeInAscendingOrder();
        }

        [Test]
        public void ApplyingIntOrderingShouldReturnProperRowsDatasOrdered()
        {
            var data = new List<RowDataModel>
            {
                new RowDataModel {Name = "Tom", Number = 2},
                new RowDataModel {Name = "Ann", Number = 0},
                new RowDataModel {Name = "Clark", Number = 3},
                new RowDataModel {Name = "Amanda", Number = 1}
            }.AsQueryable();
            var filterOrderConfiguration = new FilterOrderConfiguration()
            {
                Ordering =
                {
                    new Order {PropertyName = nameof(RowDataModel.Number), Direction = OrderDirection.Ascending}
                }
            };

            var returnedData = _sut.Apply(data, filterOrderConfiguration);

            returnedData.Select(model => model.Number).Should().BeInAscendingOrder();
        }

        [Test]
        public void ApplyingMultipleOrderingShouldReturnProperRowsDatasOrdered()
        {
            var data = new List<RowDataModel>
            {
                new RowDataModel {Name = "Tom", Number = 0},
                new RowDataModel {Name = "Amanda", Number = 2},
                new RowDataModel {Name = "Clark", Number = 1},
                new RowDataModel {Name = "Ann", Number = 3}
            }.AsQueryable();

            var expectedOrderedData = new List<RowDataModel>
            {
                new RowDataModel {Name = "Tom", Number = 0},
                new RowDataModel {Name = "Clark", Number = 1},
                new RowDataModel {Name = "Amanda", Number = 2},
                new RowDataModel {Name = "Ann", Number = 3}
            }.AsQueryable();

            var nameOrder = new Order { PropertyName = "Name", Direction = OrderDirection.Ascending };
            var numberOrder = new Order { PropertyName = "Number", Direction = OrderDirection.Ascending };
            var filterOrderConfiguration = new FilterOrderConfiguration();
            filterOrderConfiguration.Ordering.Add(nameOrder);
            filterOrderConfiguration.Ordering.Add(numberOrder);

            var returnedData = _sut.Apply(data, filterOrderConfiguration);

            returnedData.Should().BeEquivalentTo(expectedOrderedData, options => options.WithStrictOrdering());
        }

        [Test]
        public void ApplyingMixOfFilterAndOrderShouldReturnProperRowsDatasFilteredAndOrdered()
        {
            var data = new List<RowDataModel>
            {
                new RowDataModel {Name = "Abi", Number = 1},
                new RowDataModel {Name = "Clark", Number = 2},
                new RowDataModel {Name = "Ann", Number = 0},
                new RowDataModel {Name = "Amanda", Number = 3}
            }.AsQueryable();

            var expectedOrderedData = new List<RowDataModel>
            {
                new RowDataModel {Name = "Abi", Number = 1},
                new RowDataModel {Name = "Amanda", Number = 3},
                new RowDataModel {Name = "Ann", Number = 0}
            }.AsQueryable();

            var nameFilter = new Filter { PropertyName = "Name", Value = "A"};
            var nameOrder = new Order { PropertyName = "Name", Direction = OrderDirection.Ascending };
            var filterOrderConfiguration = new FilterOrderConfiguration();
            filterOrderConfiguration.Filters.Add(nameFilter);
            filterOrderConfiguration.Ordering.Add(nameOrder);

            var returnedData = _sut.Apply(data, filterOrderConfiguration);

            returnedData.Should().BeEquivalentTo(expectedOrderedData, options => options.WithStrictOrdering());
        }
    }
}
