using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookShop.Application;
using BookShop.Infrastructure;
using BookShop.Domain;
using Microsoft.Extensions.Options;
using BookShopApi.Models;
using System.IO;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using Humanizer.Bytes;
using System.Text.RegularExpressions;

namespace BookShopApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookGenreRepository _bookGenreRepository;
        private readonly IBookAuthorRepository _bookAuthorRepository;

        private readonly IConfiguration _configuration;
        private readonly IOptions<MarketSettings> _options;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger,
            ICategoryRepository categoryRepository,
            IBookRepository bookRepository,
            IConfiguration configuration,
            IAuthorRepository authorRepository,
            IBookGenreRepository bookGenreRepository,
            IBookAuthorRepository bookAuthorRepository,
            IOptions<MarketSettings> options,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _bookGenreRepository = bookGenreRepository;
            _bookAuthorRepository = bookAuthorRepository;
            _configuration = configuration;
            _options = options;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("PriceSettings")]
        public async Task<ActionResult> MinMaxPrice()
        {
            try
            {
                _logger.LogInformation("reading from json configuration complete");
                return await Task.FromResult(Ok(_options.Value.PriceSettings));
            }
            catch
            {
                _logger.LogError("reading from json configuration failed");
                return BadRequest();
            }
        }

        [HttpGet("AllGenres")]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            try
            {
                _logger.LogInformation("successful request");
                return await _categoryRepository.GetAll();
            }
            catch
            {
                _logger.LogError("request fail");
                return BadRequest();
            }
        }

        [HttpGet("NewBooks")]
        public async Task<ActionResult<List<BookGenre>>> GetBooks() // Прикольно, с List<T> работает а с IEnumerable<T> нет
        {
            try
            {
                _logger.LogInformation("successful request");
                return (List<BookGenre>)await _bookRepository.GetNewBooksWithGenres();
            }
            catch
            {
                _logger.LogError("request fail");
                return BadRequest();
            }
        }

        [HttpPost("UploadFile")]
        public async Task<ActionResult> UploadSingleFile(IFormFile file)
        {
            if (file is null)
                return BadRequest();

            string uploads = Path.Combine(_hostEnvironment.WebRootPath, "Files");
            try
            {
                string filePath = Path.Combine(uploads, file.FileName);
                using Stream fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpGet("UploadToDbFromOpenLibrary")]
        public async Task<ActionResult<string>> Jsonius(string bookName)
        {
            try // Мастер и Маргарита / Война и мир
            {
                string url = $"https://openlibrary.org/search.json?title={bookName}";
                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                //List<Root> root = JsonSerializer.Deserialize<List<Root>>(responseBody);
                Root root = JsonConvert.DeserializeObject<Root>(responseBody);

                if (root == null)
                    return BadRequest();

                // Заполняем классы данными из json
                Book book = new Book
                {
                    Title = root.docs[0].title,
                    PublishedYear = root.docs[0].publish_year[0],
                    Pages = root.docs[0].number_of_pages_median,
                    PublishName = root.docs[0].publisher[0],
                    Description = "-",
                    ImageUrl = "https://cdn.discordapp.com/attachments/528164850082381824/1097124089719697470/BookCover.png"
                };
                string[] fio = root.docs[1].author_name[0].Split(" ");
                Author author = new Author 
                {
                    FirstName = fio[0],
                    LastName = fio.Length == 2 ? fio[1] : fio[1] + " " + fio[2],
                    BirthDate = DateTime.MinValue,
                    Country = "-",
                    Description = "-",
                    ImageUrl = "#"
                };
                Genre genre = new Genre() 
                {
                    Name = TranslateGenre(root.docs[0].seed),
                    Description = "-"
                };

                // Добавляем в базу
                Book? checkBook = _bookRepository.Get(p => p.Title == book.Title &&
                                                           p.PublishedYear == book.PublishedYear &&
                                                           p.PublishName == book.PublishName)
                                                      .Result.FirstOrDefault();
                if (checkBook is null)
                    await _bookRepository.Insert(book);
                else return "Данная книга уже имеется в базе данных";

                // Если автор имеется в бд то создаем связь. если нет то добавляем автора в бд и потом уже добавляем связь
                Author? checkAuthor = _authorRepository.Get(p => p.FirstName == author.FirstName && p.LastName == author.LastName).Result.FirstOrDefault();
                if (checkAuthor is not null)
                {
                    await _bookAuthorRepository.Insert(new BookAuthor
                    {
                        Book = book,
                        Author = checkAuthor,
                    });
                }
                else
                {
                    checkAuthor = new Author
                    {
                        FirstName = author.FirstName,
                        LastName = author.LastName,
                        Description = "-",
                        ImageUrl = "https://cdn.discordapp.com/attachments/528164850082381824/1097124089719697470/BookCover.png"
                    };
                    await _authorRepository.Insert(checkAuthor);
                    await _bookAuthorRepository.Insert(new BookAuthor
                    {
                        Book = book,
                        Author = checkAuthor,
                    });
                }

                Genre? checkGenre = _categoryRepository.Get(p => p.Name == genre.Name).Result.FirstOrDefault();
                if (checkGenre is not null)
                {
                    await _bookGenreRepository.Insert(new BookGenre
                    {
                        Book = book,
                        Genre = checkGenre,
                    });
                }
                else
                {
                    checkGenre = new Genre
                    {
                        Name = genre.Name,
                        Description = "-"
                    };
                    await _categoryRepository.Insert(checkGenre);
                    await _bookGenreRepository.Insert(new BookGenre
                    {
                        Book = book,
                        Genre = checkGenre,
                    });
                }
                return Ok("Данные успешно добавлены");
            }
            catch
            {
                return BadRequest("Книга не найдена");
            }

        }

        private string TranslateGenre(List<string> data)
        {
            Dictionary<string, string> eng_rus = new Dictionary<string, string> 
            {
                {"Classic", "Классическая литература"},
                {"Detective", "Детектив"},
                {"Fantasy", "Фэнтези"},
                {"Science fiction", "Научная фантастика"},
                {"Horror", "Ужасы"},
                {"Mystic", "Мистика"},
                {"Adventures", "Приключения"},
                {"Works of fiction", "Повести"},
                {"Stories", "Рассказы"},
                {"Novels", "Романы"},
                {"Thrillers", "Триллеры"},
            };

            foreach (var item in data)
            {
                // Находим строки со списком жанров
                if(Regex.IsMatch(item, @"/subjects/(\w*)", RegexOptions.IgnoreCase))
                {
                    string genreNameEn = item.Substring(10); // Вырезаем название жанра которок идет после слэша
                    foreach (var dictItem in eng_rus)
                    {
                        if (dictItem.Key == genreNameEn.Replace(genreNameEn[0], Char.ToUpper(genreNameEn[0]))) // Делаем первую букву заглавной и сравниваем
                        {
                            return dictItem.Value;
                        }
                    }
                }
            }
            return "Другое";

        }

        private string UtfCheck(string str) 
        {
            //Console.WriteLine("\u041c\u0430\u0441\u0442\u0435\u0440 \u0438 \u041c\u0430\u0440\u0433\u0430\u0440\u0438\u0442\u0430");
            // интересно. Visual studio автоматически декодирует записи из кодов Utf
            if (str.Any(p => Char.IsAscii(p)))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                return Encoding.UTF8.GetString(bytes);
            }
            else return str;
        }

    }
}