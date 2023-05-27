using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Domain;

namespace BookShop.Application
{
    public interface IAuthorRepository : IGenericRepository<Author>
    {
        IEnumerable ToSelectListItems(IEnumerable<Author> authors, int selectedId);
    }
}
