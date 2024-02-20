using AutoMapper;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/library")]
    public class LibraryController(ILibraryService libraryService, IMapper mapper, ILogger<LibraryController> logger) : ControllerBase
    {
        /// <summary>
        /// Searches the library of books against the following fields -
        /// title, genre, type, publication year, author name
        /// </summary>
        /// <param name="searchTerm">Criteria to search with. Needs to be at least two characters.</param>
        /// <returns>A composite search result consisting of the list of books and list of authors</returns>
        [HttpGet("search")]
        [SwaggerOperation("Search")]
        [SwaggerResponse(200, "List of books and list of authors", typeof(List<Models.SearchResult>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
        public async Task<ActionResult<List<Models.SearchResult>>> SearchBooks([FromQuery] [Required] [MinLength(2)] [MaxLength(200)] string searchTerm)
        {
            try
            {
                if(searchTerm == "throw")
                {
                    throw new Exception("SearchTerm is throw");
                }

                var books = await libraryService.Search(searchTerm);

                if (books.Count == 0)
                {
                    return NoContent();
                }

                var searchedBooks = mapper.Map<List<Models.Book>>(books);
                var searchedAuthors = mapper.Map<List<Models.Author>>(books).DistinctBy(a => a.AuthorId).ToList();

                var searchResult = new Models.SearchResult { Books = searchedBooks, Authors = searchedAuthors };

                return Ok(searchResult);
            }
            catch (Exception ex){
                logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,
                "InternalServerError");
            }
        }
    }
}