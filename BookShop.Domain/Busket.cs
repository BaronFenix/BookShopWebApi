using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public class Busket : BaseEntity
    {
        public Book Books { get; set; }
        public Order Order { get; set; }
    }
}
