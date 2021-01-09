using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Helper
{
    //This class helps us in Pagination
    public class PaginationParams
    {
        //Maximum you can show 50 record per page
        private const int MaxPageSize = 50;

        //ByDefault we will show 1st page to user.
        public int PageNumber { get; set; } = 1;

        //Records per page
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;

            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
