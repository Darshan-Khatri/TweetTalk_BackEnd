using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Helper
{
    public class PaginationHeader
    {
        //These properties will be added to respose header when client class GetAllUser API.
        public PaginationHeader(int currentPage, int itemPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemPerPage = itemPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
