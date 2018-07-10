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
        private string _columnPropertyName = "Name";

        [SetUp]
        public void SetUp()
        {
            _dataProviderMock = new Mock<IFilteredDataProvider<Json>>();
            _dataProviderMock.SetupAllProperties();
            _returnedRowsFunc = () => new List<Json>();
            _dataProviderMock.Setup(provider => provider.CurrentPageRows).Returns(() => _returnedRowsFunc());
            _dataProviderMock
                .Setup(provider => provider.TotalRows)
                .Returns(() => 500);
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

            _sut.Columns.Should().HaveCount(1);
            _sut.Columns.Should().ContainSingle().Which.DisplayName.Should().Be(columnDisplayName);
            _sut.Columns.Should().ContainSingle().Which.LoadRows.Should().NotBeNull();
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
        public void AfterChangingCurrentPageIndexNewPagesAndRowsShouldBeExposed()
        {
            var newPageIndex = 2;
            InitSut();

            _sut.Pagination.Handle(new UniDataTable.PaginationViewModel.Input.CurrentPageIndex { Value = newPageIndex });

            _dataProviderMock.Object.PaginationConfiguration.CurrentPageIndex.Should().Be(newPageIndex);
            _sut.Pages.Should().HaveCount(3);
        }

        [Test]
        public void AfterCallingCurrentPageIndexHandleWitTooBigPageIndexLastPageIndexShouldBeSetInstead()
        {
            var wrongIndex = 100;
            var expectedLastPageIndex = 25;
            InitSut();

            _sut.Pagination.Handle(new UniDataTable.PaginationViewModel.Input.CurrentPageIndex { Value = wrongIndex });

            _dataProviderMock.Object.PaginationConfiguration.CurrentPageIndex.Should().Be(expectedLastPageIndex);
        }

        [Test]
        public void AfterCallingCurrentPageIndexHandleWitPageIndexLowerThanZeroZeroShouldBeSetInstead()
        {
            var wrongIndex = -1;
            var expectedIndex = 0;
            InitSut();

            _sut.Pagination.Handle(new UniDataTable.PaginationViewModel.Input.CurrentPageIndex { Value = wrongIndex });

            _dataProviderMock.Object.PaginationConfiguration.CurrentPageIndex.Should().Be(expectedIndex);
        }

        [Test]
        public void AfterCallingFilterHandleForAlreadyExistingFilterFilterValueShouldChange()
        {
            var expectedFilterValue = "John";
            _dataTableColumns.Add(new DataTableColumn { PropertyName = _columnPropertyName });
            var filter = new Filter { PropertyName = _columnPropertyName, Value = "Ann" };
            InitSut();
            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Add(filter);
            var firstNameColumn = _sut.Columns.First(x => x.PropertyName == _columnPropertyName);

            firstNameColumn.Handle(new UniDataTable.ColumnsViewModel.Input.Filter { Value = expectedFilterValue });

            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Should().ContainSingle(x => x.PropertyName == _columnPropertyName).Which.Value.Should()
                .Be(expectedFilterValue);
        }

        [Test]
        public void AfterCallingFilterHandleForNotExistingFilterNewFilterShouldBeAdded()
        {
            var filterPropertyName = "FirstName";
            var filterValue = "Ann";
            _dataTableColumns.Add(new DataTableColumn { PropertyName = _columnPropertyName });
            var filter = new Filter { PropertyName = filterPropertyName, Value = filterValue };
            InitSut();
            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Add(filter);
            var firstNameColumn = _sut.Columns.First(x => x.PropertyName == _columnPropertyName);

            firstNameColumn.Handle(new UniDataTable.ColumnsViewModel.Input.Filter { Value = "John" });

            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Should().HaveCount(2);
            _dataProviderMock.Object.FilterOrderConfiguration.Filters.Should().ContainSingle(x => x.PropertyName == filterPropertyName).Which.Value.Should()
                .Be(filterValue);
        }

        [Test]
        public void AfterCallingSortHandlerForAlreadyExistingOrderOrderValueShouldChange()
        {
            var expectedSortDirection = "desc";
            _dataTableColumns.Add(new DataTableColumn { PropertyName = _columnPropertyName });
            var order = new Order { PropertyName = _columnPropertyName, Direction = OrderDirection.Ascending };
            InitSut();
            _dataProviderMock.Object.FilterOrderConfiguration.Ordering.Add(order);
            var firstNameColumn = _sut.Columns.First(x => x.PropertyName == _columnPropertyName);

            firstNameColumn.Handle(new UniDataTable.ColumnsViewModel.Input.Sort { Value = expectedSortDirection });

            _dataProviderMock.Object.FilterOrderConfiguration.Ordering.Should().ContainSingle(x => x.PropertyName == _columnPropertyName).Which.Direction.Should()
                .Be(OrderDirection.Descending);
        }

        [Test]
        public void AfterCallingSortHandlerOrderForNotExistingOrderNewOrderShouldBeAdded()
        {
            var filterPropertyName = "FirstName";
            var expectedSortDirection = "desc";
            _dataTableColumns.Add(new DataTableColumn { PropertyName = _columnPropertyName });
            var order = new Order { PropertyName = filterPropertyName, Direction = OrderDirection.Ascending };
            InitSut();
            _dataProviderMock.Object.FilterOrderConfiguration.Ordering.Add(order);
            var firstNameColumn = _sut.Columns.First(x => x.PropertyName == _columnPropertyName);

            firstNameColumn.Handle(new UniDataTable.ColumnsViewModel.Input.Sort { Value = expectedSortDirection });

            _dataProviderMock.Object.FilterOrderConfiguration.Ordering.Should().ContainSingle(x => x.PropertyName == filterPropertyName).Which.Direction.Should()
                .Be(OrderDirection.Ascending);
        }

        [Test]
        public void AfterCallingSortHandlerWithInvalidSortValue()
        {
            _dataTableColumns.Add(new DataTableColumn { PropertyName = _columnPropertyName });
            InitSut();
            var firstNameColumn = _sut.Columns.First(x => x.PropertyName == _columnPropertyName);
            _returnedRowsFunc = () => new List<RowViewModel>
            {
                new RowViewModel {Name = "Clark"},
                new RowViewModel {Name = "Ann"}
            };

            firstNameColumn.Handle(new UniDataTable.ColumnsViewModel.Input.Sort { Value = "Test" });

            _dataProviderMock.Object.FilterOrderConfiguration.Ordering.Should().BeEmpty();
        }
    }
}