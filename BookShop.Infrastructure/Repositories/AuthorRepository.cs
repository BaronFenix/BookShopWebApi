using BookShop.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Domain;
using System.Collections;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Infrastructure.Repositories
{
    public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
    {
        private readonly ApplicationContext _context;
        public AuthorRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable ToSelectListItems(IEnumerable<Author> authors, int selectedId)
        {
            return authors.Select(p =>
                          new SelectListItem
                          {
                              Selected = (p.Id == selectedId),
                              Text = p.FirstName + " " + p.LastName,
                              Value = p.Id.ToString()
                          });
        }
    }
}
