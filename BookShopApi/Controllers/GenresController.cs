using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookShop.Infrastructure;
using BookShop.Application;
using BookShop.Domain;
using System.Collections.Generic;
using System.Linq;

namespace BookShopApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly ILogger<GenresController> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public GenresController(
            IBookRepository bookRepository,
            ILogger<GenresController> logger,
            ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet(Name = "GetGenres")]
        public async Task<ActionResult<IEnumerable<Genre>>> Index()
        {
            try
            {
                _logger.LogInformation("successull request");
                return Ok(await _categoryRepository.GetAll());
            }
            catch
            {
                _logger.LogError("Request fail");
                return BadRequest();
            }
        }

        [HttpGet("{id}", Name = "GetGenreById")]
        public async Task<ActionResult<Genre>> Details(int id)
        {
            try
            {
                _logger.LogInformation("successull request");
                return Ok(await _categoryRepository.GetById(id));
            }
            catch
            {
                _logger.LogError("Request fail");
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Genre>> Create(Genre newGenre)
        {
            IEnumerable<Genre> genres = await _categoryRepository.GetAll();

            if (genres == null)
            {
                _logger.LogWarning("object is null");
                return BadRequest();
            }

            await _categoryRepository.Insert(newGenre);
            _logger.LogInformation("successfully added");

            return Ok(newGenre);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Genre>> Edit(Genre editedGenre, IFormCollection collection)
        {
            try
            {
                await _categoryRepository.Update(editedGenre);
                _logger.LogInformation("successfully updated");

                return Ok();
            }
            catch
            {
                _logger.LogError("failed to update");
                return BadRequest();
            }
        }


        [HttpDelete("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await _categoryRepository.Delete(id);
                _logger.LogInformation("successfully deleted");
                return Ok();
            }
            catch
            {
                _logger.LogError("Failed to delete");
                return BadRequest();
            }

        }
    }
}
