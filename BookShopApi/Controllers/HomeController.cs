using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookShop.Application;
using BookShop.Infrastructure;
using BookShop.Domain;
using Microsoft.Extensions.Options;
using BookShopApi.Models;
using System.IO;

namespace BookShopApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        private readonly IConfiguration _configuration;
        private readonly IOptions<PriceSettings> _options;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnvironment;

        public HomeController(ILogger<HomeController> logger,
            ICategoryRepository categoryRepository,
            IBookRepository bookRepository,
            IConfiguration configuration,
            IOptions<PriceSettings> options,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
            _configuration = configuration;
            _options = options;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("PriceSettings")]
        public async Task<ActionResult> MinMaxPrice()
        {
            return Ok(_options.Value);
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

       

    }
}