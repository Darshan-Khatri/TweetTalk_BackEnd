using DatingApplicationBackEnd.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Persistance
{
    public class DataContext : DbContext
    {
        //Note:- DbContextOptions dataType MUST BE SAME AS OUR class name(DataContext). If you write "DbContext" bymistake instead of className(DataContext) then it will cause a error.
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<AppUser> Users { get; set; }

        //Take Aways:- You should be very careful while writing datatypes.
    }
}
