using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Helper
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            /*For example after query we got 20 records = count,
             * each page contains 5 record = pageSize,
             * so the Total number of pages requried to display all records is 20/5 = 4
             */
            CurrentPage = pageNumber;

            //Total number of pages requried to display all the records based on pageSize
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            //Number of records in each page, for ex pageSize = 5, means we have 5 records per page.
            PageSize = pageSize;

            //Total number of records in database
            TotalCount = count;

            //We are getting list of specified record because of below query
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }

        //How many items are in this query
        //i.e. Filter is for Gender female so how many females are in list
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            //This calls our database and get the total number of record in from table
            var count = await source.CountAsync();

            //If we are on page number 2 and pageSize is 5, so we gonna skip first five records and then will take next 5 records
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
