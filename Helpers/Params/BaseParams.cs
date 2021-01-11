namespace dotnet.boilerplate.Helpers.Params
{
    public class BaseParams
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public string FilterBy { get; set; }

        public string SortBy { get; set; }

        public string OrderBy { get; set; }

        public bool isDescending { get; set; } = false;
    }
}