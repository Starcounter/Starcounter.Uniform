﻿using System.Linq;
using Starcounter.Uniform.Builder;
using Starcounter.Uniform.Generic.Pagination;
using Starcounter.Uniform.Queryables;

namespace Starcounter.Uniform.Generic.FilterAndSort
{
    /// <summary>
    /// Provides data to the data table control.
    /// Usually the app developer can use <see cref="FilteredPaginatedDataProvider{TData,TViewModel}"/>, created using <see cref="DataTableBuilder{TViewModel}"/>.
    /// This interface can be implemented to expose data that is not <see cref="IQueryable"/>.
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public interface IFilteredDataProvider<out TViewModel> : IPaginatedDataProvider<TViewModel>
    {
        /// <summary>
        /// Set it, or change its properties to change the value of <see cref="IPaginatedDataProvider{TViewModel}.CurrentPageRows"/>
        /// </summary>
        /// <remarks>Implementers MUST ignore this property if it's null. This is to preserve the Liskov Substitution Principle</remarks>
        FilterOrderConfiguration FilterOrderConfiguration { get; set; }
    }
}