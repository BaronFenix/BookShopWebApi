using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public class BaseEntity 
    {
        [Key]
        public int Id { get; set; }
    }

    public class BaseDateEntity : BaseEntity
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

}
