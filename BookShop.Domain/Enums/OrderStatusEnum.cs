using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public enum OrderStatusEnum
    {
        Created = 0,
        Paying = 1,
        Payed = 2, 
        Cancelled = 3,
        Completed = 4

    }
}
