using BookShop.Application;
using BookShop.Domain;
using BookShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Infrastructure
{
    public class StoreService : IStoreService
    {
        internal DbContext context;

        private BookRepository bookRepository;
        private CategoryRepository categoryRepository;
        private BusketRepository busketRepository;


        public StoreService(DbContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddToBusket(Order order, Book book, Price price)
        {
            //DbSet<Busket> dbSet = context.Set<Busket>();
            
            busketRepository.context.FindAsync<Busket>(0);


            using (ApplicationContext appCont = new ApplicationContext())
            {
                GenericRepository<Busket> busketRep = new BusketRepository(appCont);

                var busket = from tmp in appCont.Buskets
                             where tmp.Order.Id == order.Id
                             select tmp;


                if(!await busketRep.dbSet.AnyAsync(p => p.Id == book.Id && p.Order.Id == order.Id))
                {
                    await busketRep.Insert(new Busket {Order = order, Books = book});
                }



                await busketRep.Insert(new Busket { Books = book, Order = order });
            }
            

            return await Task.FromResult(true);
        }

        public Task<Order> CreateOrUpdateOrder(int bookId, string userName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFromBusket(Order order, int bookId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Book> FindBooks(string title)
        {
            throw new NotImplementedException();
        }

        public Order FindOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Author>> GetAuthor()
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetBookById(int bookId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetBooks()
        {
            throw new NotImplementedException();
        }

        public Order GetLastUnpayedOrder(string UserName)
        {
            throw new NotImplementedException();
        }
    }
}
