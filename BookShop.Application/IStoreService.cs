using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Domain;

namespace BookShop.Application
{
    public interface IStoreService
    {
        Task<Book> GetBookById(int bookId);
        Task<Order> CreateOrUpdateOrder(int bookId, string userName);
        Task<bool> AddToBusket(Order order, Book book, Price price);
        bool DeleteFromBusket(Order order, int bookId);
        Order FindOrder(int orderId);
        IEnumerable<Book> FindBooks(string title);
        Order GetLastUnpayedOrder(string UserName);
        Task<IEnumerable<Book>> GetBooks();
        Task<IEnumerable<Author>> GetAuthor();
    }
}
