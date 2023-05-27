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
    public class PriceRepository : GenericRepository<Price>, IPriceRepository
    {
        private readonly ApplicationContext _context;
        public PriceRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Price> GetByBookId(int bookId)
        {
            return await _context.Prices.Where(p => p.Book.Id == bookId).FirstOrDefaultAsync();
        }
    }
}
