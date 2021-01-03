using DatingApplicationBackEnd.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Extensions
{
    //This class is used to add pagination information inside the response header from server to client, So client can use that header and perform their task
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));

            /*To allow our corse polici pass header in respose we have to specify "Access-Control-Expose-Header" exactly same way, if you do spelling mistake then it will cause error.
             */
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
