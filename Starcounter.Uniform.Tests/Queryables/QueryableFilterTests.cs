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
    }
}
