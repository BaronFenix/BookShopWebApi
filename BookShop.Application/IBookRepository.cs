using BookShop.Domain;

namespace BookShop.Application
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksFromCategory(int CategoryId);
        Task<IEnumerable<BookGenre>> GetNewBooksWithGenres();
        Task<List<FullBookInfo>> GetFullData();
    }
}