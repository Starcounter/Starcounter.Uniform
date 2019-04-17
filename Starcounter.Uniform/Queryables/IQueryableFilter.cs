using System;
using System.Linq;
using System.Linq.Expressions;
using Starcounter.Uniform.Generic.FilterAndSort;

namespace Starcounter.Uniform.Queryables
{
    public interface IQueryableFilter<TData>
    {
        Expression<Func<IQueryable<TData>>> Apply(Expression<Func<IQueryable<TData>>> data,
            FilterOrderConfiguration configuration);
    }
}