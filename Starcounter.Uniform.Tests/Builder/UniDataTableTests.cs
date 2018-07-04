﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starcounter.Uniform.Tests.Builder
{
    public class UniDataTableTests
    {
        private UniDataTable _sut;
        private Mock<IFilteredDataProvider<Json>> _dataProviderMock;
        private List<DataTableColumn> _dataTableColumns;
        private int _initialPageSize;
        private int _initialPageIndex;
        private Func<IReadOnlyCollection<Json>> _returnedRowsFunc;

        [SetUp]
        public void SetUp()
        {
            _dataProviderMock = new Mock<IFilteredDataProvider<Json>>();
            _dataProviderMock.SetupAllProperties();
            _returnedRowsFunc = () => new List<Json>();
            _dataProviderMock.Setup(provider => provider.CurrentPageRows).Returns(() => _returnedRowsFunc());
            _dataTableColumns = new List<DataTableColumn>();
            _initialPageSize = 20;
            _initialPageIndex = 0;
        }

        private void InitSut()
        {
            _sut = new UniDataTable().Init(_dataProviderMock.Object, _dataTableColumns, _initialPageSize, _initialPageIndex);
        }

        [Test]
        public void AfterInitSpecifiedColumnShouldBePresent()
        {
            var columnDisplayName = "First name";
            _dataTableColumns.Add(new DataTableColumn { DisplayName = columnDisplayName });

            InitSut();

            _sut.Columnns.Should().HaveCount(1);
            _sut.Columnns.Should().ContainSingle().Which.DisplayName.Should().Be(columnDisplayName);
            _sut.Columnns.Should().ContainSingle().Which.LoadRows.Should().NotBeNull();
        }

        [Test]
        public void AfterInitInitialPageShouldBeLoaded()
        {
            var expectedRow = new Json();
            var expectedRowsCollection = new List<Json>
            {
                expectedRow
            };
            var initialPageIndex = 3;
            _initialPageIndex = initialPageIndex;
            _dataProviderMock
                .Setup(provider => provider.CurrentPageRows)
                .Returns(() =>
                _dataProviderMock.Object.PaginationConfiguration
                    .CurrentPageIndex == initialPageIndex
                    ? expectedRowsCollection
                    : new List<Json>());

            InitSut();

            _sut.Pages.Should().HaveCount(initialPageIndex + 1);
            _sut.Pages[initialPageIndex].Rows.Should().ContainSingle().Which.Should().BeSameAs(expectedRow);
        }

        [Test]
        public void AfterChangingCurrentPageIndexPaginationConfigurationShouldChange()
        {
            var newPageIndex = 2;

            InitSut();
            _sut.Pagination.Handle(new UniDataTable.PaginationViewModel.Input.CurrentPageIndex { Value = newPageIndex });

            _dataProviderMock.Object.PaginationConfiguration.CurrentPageIndex.Should().Be(newPageIndex);
            _dataProviderMock.VerifyGet(x => x.CurrentPageRows, Times.Exactly(2));
        }

        [Test]
        public void AfterCallingFilterHandleFilerValueInFilterOrderConfigurationShouldChange()
        {
            var propertyName = "Name";
            var expectedFilterValue = "John";
            _dataTableColumns.Add(new DataTableColumn { PropertyName = propertyName });
            var filter = new Filter { PropertyName = propertyName, Value = "Ann" };

            InitSut();
            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Add(filter);
            var firstNameColumn = _sut.Columnns.First(x => x.PropertyName == propertyName);
            firstNameColumn.Handle(new UniDataTable.ColumnsViewModel.Input.Filter { Value = expectedFilterValue });

            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Should().ContainSingle().Which.Value.Should()
                .Be(expectedFilterValue);
        }

        [Test]
        public void AfterCallingFilterHandleForNotExistingFilterNewFilterShouldBeAddedToFilterOrderConfiguration()
        {
            var columnName = "Test";
            var propertyName = "Name";
            var filterValue = "Ann";
            _dataTableColumns.Add(new DataTableColumn { PropertyName = columnName });
            var filter = new Filter { PropertyName = propertyName, Value = filterValue };

            InitSut();
            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Add(filter);
            var firstNameColumn = _sut.Columnns.First(x => x.PropertyName == columnName);
            firstNameColumn.Handle(new UniDataTable.ColumnsViewModel.Input.Filter { Value = "John" });

            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Should().ContainSingle(x => x.PropertyName == propertyName).Which.Value.Should()
                .Be(filterValue);
        }
    }
}
