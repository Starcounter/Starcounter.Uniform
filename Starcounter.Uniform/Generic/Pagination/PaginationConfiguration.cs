namespace Starcounter.Uniform.Generic.Pagination
{
    public class PaginationConfiguration
    {
        public int PageSize { get; set; }
        public int CurrentPageIndex { get; set; }

        public PaginationConfiguration(int pageSize, int currentPageIndex = 0)
        {
            this.PageSize = pageSize;
            this.CurrentPageIndex = currentPageIndex;
        }
    }
}

