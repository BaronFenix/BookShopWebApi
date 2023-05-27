using BookShop.Application;
using BookShop.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly ApplicationContext _context;
        public BookRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetBooksFromCategory(int CategoryId)
        {
            return await _context.BookGenres
                .Include(b => b.Book)
                .Where(g => g.Genre.Id == CategoryId)
                .Select(p => p.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookGenre>> GetNewBooksWithGenres()
        {
            return await _context.BookGenres
                .Include(b => b.Book)
                .Include(g => g.Genre)
                .OrderByDescending(p => p.Book.PublishedYear)
                .ToListAsync();
        }

        public async Task<List<FullBookInfo>> GetFullData()
        {
            var result = from book in _context.Books
                         join genre in _context.BookGenres on book.Id equals genre.Book.Id
                         join author in _context.BookAuthors on book.Id equals author.Book.Id
                         select new FullBookInfo
                         {
                             Book = book,
                             Genre = genre.Genre,
                             Author = author.Author,
                             Price = (from tmp in _context.Prices
                                     where tmp.Book.Id == book.Id
                                     select tmp).Single()
                         };
            return await result.ToListAsync();
        }
    }
}
