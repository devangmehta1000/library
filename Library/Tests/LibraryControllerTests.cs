using AutoMapper;
using Library.Controllers;
using Library.Mapper;
using Library.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class LibraryControllerTests
    {
        private LibraryController _libraryController;
        private Mock<ILibraryService> _libraryService;
        private IMapper _mapper;

        public LibraryControllerTests()
        {
            _libraryService = new Mock<ILibraryService>();
            var logger = new Mock<ILogger<LibraryController>>();

            // Setup AutoMapper
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); 
                cfg.ConstructServicesUsing(t =>
                {
                    if (t.UnderlyingSystemType == typeof(BookNameResolver))
                        return new BookNameResolver();
                    if (t.UnderlyingSystemType == typeof(AgeResolver))
                        return new AgeResolver();
                    return null;
                });
            });

            _mapper = configuration.CreateMapper();

            // initialise controller
            _libraryController = new LibraryController(_libraryService.Object, _mapper, logger.Object);
        }

        private List<Services.Models.Book> GetBooks()
        {
            return new List<Services.Models.Book>
            {
                new Services.Models.Book { BookId = 1, Title = "Cranky Bear", PublicationYear = 2000, Type = "Childrens Book", Genre = "Picture Book", AuthorId = 2, Author = new Services.Models.Author { AuthorId = 2, FirstName = "Alice", LastName = "Chapman", Gender = "Female", DateOfBirth = new DateTime(1975, 8, 5), Books = new List<Services.Models.Book>{ new  Services.Models.Book { BookId = 1, Title = "Cranky Bear", PublicationYear = 2000, Type = "Childrens Book", Genre = "Picture Book", AuthorId = 2} } } },
                new Services.Models.Book { BookId = 2, Title = "Mighty Elephant", PublicationYear = 2001, Type = "Childrens Book", Genre = "Picture Book", AuthorId = 2, Author = new Services.Models.Author { AuthorId = 2, FirstName = "Alice", LastName = "Chapman", Gender = "Female", DateOfBirth = new DateTime(1975, 8, 5) } },
                new Services.Models.Book { BookId = 3, Title = "Let's start ABC", PublicationYear = 2002, Type = "Childrens Book", Genre = "Early Learners", AuthorId = 2, Author = new Services.Models.Author { AuthorId = 2, FirstName = "Alice", LastName = "Chapman", Gender = "Female", DateOfBirth = new DateTime(1975, 8, 5) } },
                new Services.Models.Book { BookId = 4, Title = "Down the Road", PublicationYear = 2004, Type = "Fiction", Genre = "Romance", AuthorId = 3, Author = new Services.Models.Author { AuthorId = 3, FirstName = "Emma", LastName = "Wright", Gender = "Female", DateOfBirth = new DateTime(1984, 11, 17) } },
                new Services.Models.Book { BookId = 6, Title = "Australian Diaries", PublicationYear = 1982, Type = "Non-Fiction", Genre = "Travel", AuthorId = 1, Author = new Services.Models.Author { AuthorId = 1, FirstName = "Joe", LastName = "Blogs", Gender = "Male", DateOfBirth = new DateTime(1960, 3, 29) } },
                new Services.Models.Book { BookId = 7, Title = "French Diaries", PublicationYear = 1983, Type = "Non-Fiction", Genre = "Travel", AuthorId = 1, Author = new Services.Models.Author { AuthorId = 1, FirstName = "Joe", LastName = "Blogs", Gender = "Male", DateOfBirth = new DateTime(1960, 3, 29) } },
                new Services.Models.Book { BookId = 8, Title = "Brazilian Diaries", PublicationYear = 1984, Type = "Non-Fiction", Genre = "Travel", AuthorId = 1, Author = new Services.Models.Author { AuthorId = 1, FirstName = "Joe", LastName = "Blogs", Gender = "Male", DateOfBirth = new DateTime(1960, 3, 29) } },
                new Services.Models.Book { BookId = 9, Title = "2000 - Year of Y2K", PublicationYear = 2005, Type = "Science", Genre = "Technology", AuthorId = 4, Author = new Services.Models.Author { AuthorId = 4, FirstName = "Charlie", LastName = "Smith", Gender = "Male", DateOfBirth = new DateTime(1980, 5, 21) } }
            };
        }

        [Fact]
        public async Task SearchBooks_ValidResult()
        {
            var searchTerm = "cranky";
            var searchedBooks = GetBooks().Where(b => b.BookId == 1).ToList();
           _libraryService.Setup(repo => repo.Search(searchTerm)).ReturnsAsync(searchedBooks);

            // Act
            var result = await _libraryController.SearchBooks(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var searchResult = Assert.IsType<Controllers.Models.SearchResult>(okResult.Value);
            Assert.Equal(searchedBooks.First().BookId, searchResult.Books.First().BookId);
            Assert.Equal(searchedBooks.First().Title, searchResult.Books.First().Title);
            Assert.Equal(searchedBooks.First().PublicationYear, searchResult.Books.First().PublicationYear);
            Assert.Equal(searchedBooks.First().Type, searchResult.Books.First().Type);
            Assert.Equal(searchedBooks.First().Genre, searchResult.Books.First().Genre);
            Assert.Equal(searchedBooks.First().Author.Gender, searchResult.Authors.First().Gender);

            // test bookname resolvers
            Assert.Contains(searchedBooks.First().Author.FirstName, searchResult.Books.First().AuthorName);
            Assert.Contains(searchedBooks.First().Author.LastName, searchResult.Books.First().AuthorName);
            Assert.Contains(searchedBooks.First().Author.FirstName, searchResult.Authors.First().FullName);
            Assert.Contains(searchedBooks.First().Author.LastName, searchResult.Authors.First().FullName);

            // test age resolver
            Assert.Equal(48, searchResult.Authors.First().Age);
        }

        [Fact]
        public async Task SearchBooks_NoContent()
        {
            var searchTerm = "does-not-exist";
            _libraryService.Setup(repo => repo.Search(searchTerm)).ReturnsAsync(new List<Services.Models.Book>());            

            // Act
            var result = await _libraryController.SearchBooks(searchTerm);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task SearchBooks_Error()
        {
            var searchTerm = "throw";
            _libraryService.Setup(repo => repo.Search(searchTerm)).ReturnsAsync(new List<Services.Models.Book>());

            // Act
            var result = await _libraryController.SearchBooks(searchTerm);

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, objResult.StatusCode); 
            Assert.Equal("InternalServerError", objResult.Value);
        }
    }
}