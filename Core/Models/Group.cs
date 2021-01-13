using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApplicationBackEnd.Core.Models
{
    public class Group
    {
        public Group()
        {
            /*When entity framework creates new table it needs empty constructor otherwise you will get error.*/
        }

        public Group(string name)
        {
            Name = name;
        }

        [Key]
        //This is group name for chat between 2 user and it is unique.
        public string Name { get; set; }

        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}
