using BookShop.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Infrastructure.Repositories
{
    public class BookGenreRepository : GenericRepository<BookGenre>, IBookGenreRepository
    {
        private readonly ApplicationContext _context;

        public BookGenreRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<BookGenre> GetByBookId(int bookId)
        {
            return await _context.BookGenres.Where(p => p.Book.Id == bookId).Include(p => p.Genre).SingleAsync();
        }
    }
}
