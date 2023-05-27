using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Domain.Dto
{
    public class CreateAuthorDto
    {
        [Required(ErrorMessage = "Невалидное наименование")]
        [Display(Name = "Краткое наименование")]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Невалидное имя")]
        [Display(Name = "Имя")]
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Невалидная фамилия")]
        [Display(Name = "Фамилия")]
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }

        public int DeathAge { get; set; }

    }
}
