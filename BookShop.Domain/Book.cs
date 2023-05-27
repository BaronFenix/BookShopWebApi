using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public int Pages { get; set; }
        public string Description { get; set; }
        public int PublishedYear { get; set; }
        public string PublishName { get; set; }
        public string ImageUrl { get; set; }
    }
}
