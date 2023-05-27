using BookShop.Application;
using BookShop.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Infrastructure.Repositories
{
    public class BookAuthorRepository : GenericRepository<BookAuthor>, IBookAuthorRepository
    {
        private readonly ApplicationContext _context;
        public BookAuthorRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<BookAuthor> GetByBookId(int bookId)
        {
            return await _context.BookAuthors.Where(p => p.Book.Id == bookId).Include(p => p.Author).SingleAsync();
        }
    }
}
