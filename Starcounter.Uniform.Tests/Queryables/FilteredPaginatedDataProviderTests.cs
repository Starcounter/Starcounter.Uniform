using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Starcounter.Uniform.Generic.FilterAndSort;
using Starcounter.Uniform.Generic.Pagination;
using Starcounter.Uniform.Queryables;

namespace Starcounter.Uniform.Tests.Queryables
{
    public class FilteredPaginatedDataProviderTests
    {
        private List<DisposableViewModel> _createdViewModels;
        private FilteredPaginatedDataProvider<DatabaseType, DisposableViewModel> _sut;

        [SetUp]
        public void SetUp()
        {
            _createdViewModels = new List<DisposableViewModel>();
            _sut = new FilteredPaginatedDataProvider<DatabaseType, DisposableViewModel>(
                new QueryableFilter<DatabaseType>(),
                new QueryablePaginator<DatabaseType, DisposableViewModel>(),
                new[] { new DatabaseType() }.AsQueryable(),
                db =>
                {
                    var viewModel = new DisposableViewModel();
                    _createdViewModels.Add(viewModel);
                    return viewModel;
                })
            {
                FilterOrderConfiguration = new FilterOrderConfiguration(),
                PaginationConfiguration = new PaginationConfiguration(1, 0)
            };
        }

        [Test]
        public void AfterLoadingRows_ShouldDisposeOfOldRows()
        {
            // arrange - initial load of rows
            var currentRows = _sut.CurrentPageRows;

            // act - reloading rows
            currentRows = _sut.CurrentPageRows;

            // assert
            _createdViewModels.Except(currentRows).Should().OnlyContain(model => model.Disposed);
        }

        [Test]
        public void DisposeShouldDisposeOfOldRows()
        {
            // arrange - initial load of rows
            var _ = _sut.CurrentPageRows;

            // act - reloading rows
            _sut.Dispose();

            // assert
            _createdViewModels.Should().OnlyContain(model => model.Disposed);
        }

        [Test]
        public void CountingRowsShouldNotCreateViewModels()
        {
            var _ = _sut.TotalRows;

            _createdViewModels.Should().BeEmpty();
        }

        private class DatabaseType
        {
        }

        private class DisposableViewModel : Json, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
               Disposed = true;
            }
        }
    }
}