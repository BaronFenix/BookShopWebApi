using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public class Genre : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
