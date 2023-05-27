using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public class BookOrder : BaseEntity
    {
        public Book Book { get; set; }
        public Order Order { get; set; }
    }
}
