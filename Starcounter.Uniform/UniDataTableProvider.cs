using System.Collections.Generic;
using System.Linq;
using Starcounter.Linq;
using Starcounter.Uniform.Interfaces;

namespace Starcounter.Uniform
{
    public class UniDataTableProvider<T> : IUniDataTableProvider where T : class
    {
        public UniDataTableColumn[] Columns { get; protected set; }
        public int PageSize { get; set; }
        public int Page { get; set; }

        public UniDataTableProvider(UniDataTableColumn[] columns, int initPageSize, int initPage = 0)
        {
            this.Columns = columns;
            this.PageSize = initPageSize;
            this.Page = initPage;
        }

        public IEnumerable<object> SelectRows()
        {
            return DbLinq.Objects<T>().Skip(Page * PageSize).Take(PageSize);
        }

        public int CountRows()
        {
            return DbLinq.Objects<T>().Count();
        }
    }
}
