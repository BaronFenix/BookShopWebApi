using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public class Price : BaseDateEntity
    {
        public Book Book { get; set; }
        public decimal Total { get; set; }

    }
}
