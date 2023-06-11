using Microsoft.AspNetCore.Mvc;
using BookShop.Application;
using BookShop.Domain;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BookShopApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<GenresController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IBookAuthorRepository _bookAuthorRepository;
        private readonly IPriceRepository _priceRepository;

        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _memoryCache; 

        public BooksController(
            ILogger<GenresController> logger,
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IAuthorRepository authorRepository,
            IBookGenreRepository bookGenreRepository,
            IBookAuthorRepository bookAuthorRepository,
            IPriceRepository priceRepository,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _bookGenreRepository = bookGenreRepository;
            _bookAuthorRepository = bookAuthorRepository;
            _priceRepository = priceRepository;
            _hostingEnvironment = hostEnvironment;
            _memoryCache = memoryCache;
        }

        [HttpGet("{id}", Name = "GetBookById")]
        public async Task<ActionResult<Book>> GetById(int id)
        {
            bool productIsGet = _memoryCache.TryGetValue(id, out Book? memoryProduct); // Ищем книгу в ОЗУ

            if (productIsGet) // Если она найдена сразу возвращаем
                return Ok(_memoryCache.Get(id));

            // если нет, то вытаскиваем ее из базы и записываем в озу
            Book book = await _bookRepository.GetById(id); 
            if (book != null)
            {
                _memoryCache.Set(book.Id, book, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                return Ok(_memoryCache.Get(id));
            }
            else return NotFound();
        }


        [HttpGet("GetPrices")]
        public async Task<ActionResult<List<Price>>> Price()
        {
            try
            {
                _logger.LogInformation("Successuful request");
                return Ok(await _priceRepository.Get(null, null, "Book"));
            }
            catch
            {
                _logger.LogError("Request fail");
                return BadRequest();
            }
        } 

        [HttpGet("UploadBooksToDbFromFile")]
        public async Task<ActionResult> BookParser(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Files");
                filePath += $"\\{fileName}.csv";
                List<string> data;
                try
                {
                    data = System.IO.File.ReadAllLines(filePath).ToList();
                    data.RemoveAt(0); // Убираем строку с названиями столбцов
                }
                catch
                {
                    return BadRequest(new {  ErrorText = "Файл не найден" });
                }

                List<Book> CsvBooks = new List<Book>();
                List<Author> CsvAuthors = new List<Author>();
                List<string> CsvGenres = new List<string>();
                int counter = 0;

                foreach (string row in data)
                {
                    List<string> dataField = row.Replace("\"", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    // Парсим файл раскидывая данные по нужным классам
                    // Не очень хороший способ т.к. он привязан к строгой последовательности данных в файле. но что есть то есть
                    CsvBooks.Add(new Book
                    {
                        Title = dataField[0],
                        Description = dataField[1],
                        Pages = Convert.ToInt32(dataField[2]),
                        PublishedYear = Convert.ToInt32(dataField[3]),
                        PublishName = dataField[4],
                        ImageUrl = dataField[5],
                    });
                    CsvGenres.Add(dataField[6]);
                    CsvAuthors.Add(new Author
                    {
                        FirstName = dataField[7],
                        LastName = dataField[8],
                    });
                }

                // Проверяем есть ли в базе определенный жанр/автор по известным полям
                // Если есть делаем соответсвующую привязку. Если нет то сначало добавляем а потом уже создаем связь
                for (int i = 0; i < CsvBooks.Count; i++)
                {
                    Book? checkBook = _bookRepository.Get(p => p.Title == CsvBooks[i].Title &&
                                                          p.PublishedYear == CsvBooks[i].PublishedYear &&
                                                          p.PublishName == CsvBooks[i].PublishName)
                                                     .Result.FirstOrDefault();
                    if (checkBook is null)
                    {
                        await _bookRepository.Insert(CsvBooks[i]);
                        counter++;
                    }
                    else continue; // Если в базе уже есть книга совпадающая по трем параметрам то переходим к след. книге

                    Author? checkAuthor = _authorRepository.Get(p => p.FirstName == CsvAuthors[i].FirstName && p.LastName == CsvAuthors[i].LastName).Result.FirstOrDefault();
                    if (checkAuthor is not null)
                    {
                        await _bookAuthorRepository.Insert(new BookAuthor
                        {
                            Book = CsvBooks[i],
                            Author = checkAuthor,
                        });
                    }
                    else
                    {
                        checkAuthor = new Author
                        {
                            FirstName = CsvAuthors[i].FirstName,
                            LastName = CsvAuthors[i].LastName,
                            Description = "-",
                            ImageUrl = "https://cdn.discordapp.com/attachments/528164850082381824/1097124089719697470/BookCover.png"
                        };
                        await _authorRepository.Insert(checkAuthor);
                        await _bookAuthorRepository.Insert(new BookAuthor
                        {
                            Book = CsvBooks[i],
                            Author = checkAuthor,
                        });
                    }

                    Genre? checkGenre = _categoryRepository.Get(p => p.Name == CsvGenres[i]).Result.FirstOrDefault();
                    if (checkGenre is not null)
                    {
                        await _bookGenreRepository.Insert(new BookGenre
                        {
                            Book = CsvBooks[i],
                            Genre = checkGenre,
                        });
                    }
                    else
                    {
                        checkGenre = new Genre
                        {
                            Name = CsvGenres[i],
                            Description = "-"
                        };
                        await _categoryRepository.Insert(checkGenre);
                        await _bookGenreRepository.Insert(new BookGenre
                        {
                            Book = CsvBooks[i],
                            Genre = checkGenre,
                        });
                    }
                }

                _logger.LogInformation("Данные успешно записаны в БД. \nКоличество добавленных книг: {0}", counter);
                return Ok($"Запрос успешно выполнен. Количество добавленных книг: {counter}");

            }
            catch (Exception ex)
            {
                _logger.LogError("Request fail", ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("UploadPricesToDbFromFile")]
        public async Task<ActionResult> PriceParses(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Files");
                filePath += $"\\{fileName}.csv";
                List<string> data;
                try
                {
                    data = System.IO.File.ReadAllLines(filePath).ToList();
                    data.RemoveAt(0); // Убираем строку с названиями столбцов
                }
                catch
                {
                    return BadRequest(new { ErrorText = "Файл не найден" });
                }

                List<decimal> csvPrices = new List<decimal>();
                List<Book> csvBooks = new List<Book>();

                foreach (string row in data)
                {
                    List<string> dataField = row.Replace("\"", "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    csvBooks.Add(new Book
                    {
                        Title = dataField[0],
                        PublishedYear = Convert.ToInt32(dataField[1]),
                        PublishName = dataField[2],
                        Description = "-",
                        Pages = 0,
                        ImageUrl = "#"
                    });
                    csvPrices.Add(Convert.ToDecimal(dataField[3]));
                }

                for (int i = 0; i < csvBooks.Count; i++)
                {
                    Book? book = _bookRepository.Get(p =>
                                                        p.Title == csvBooks[i].Title &&
                                                        p.PublishedYear == csvBooks[i].PublishedYear &&
                                                        p.PublishName == csvBooks[i].PublishName
                                                    ).Result.FirstOrDefault();
                    if (book == null)
                        continue;

                    Price bookPrice = await _priceRepository.GetByBookId(book.Id);
                    if (bookPrice != null)
                    {
                        if (bookPrice.Total == csvPrices[i])
                            continue;
                        bookPrice.Total = csvPrices[i];
                        await _priceRepository.Update(bookPrice);
                    }
                    else
                    {
                        await _priceRepository.Insert(new Price
                        {
                            Book = book,
                            Total = csvPrices[i]
                        });
                    }

                }
                _logger.LogInformation("Successful request");
                return Ok("Цены успешно добавлены/обновлены");
            }
            catch(Exception ex)
            {
                _logger.LogError("Request fail");
                return BadRequest(ex.Message);
            }
        }

    }
}
