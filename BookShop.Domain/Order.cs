using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Domain
{
    public class Order : BaseDateEntity
    {
        [Display(Name = "Пользователь")]
        public User UserName { get; set; }

        [Display(Name = "Статус")]
        public OrderStatusEnum Status { get; set; }

        [Display(Name = "К оплате")]
        public decimal Total { get; set; }
    }
}
