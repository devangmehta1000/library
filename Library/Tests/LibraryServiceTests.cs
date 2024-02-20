using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Library.Data;
using Library.Services;
using Library.Mapper;
using Moq;

namespace Library.Tests
{
    public class LibraryServiceTests
    {
        private LibraryService _libraryService;
        private IMapper _mapper;
        private Mock<IConfiguration> _config;

        public LibraryServiceTests()
        {
            // Setup AutoMapper
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configuration.CreateMapper();

            // Setup DbContext (use in-memory database for testing)
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase("LibrarySystem")
                .Options;

            var context = new LibraryContext(options);

            _config = new Mock<IConfiguration>();
            _config.Setup(config => config["Fuzzy:Tolerance"]).Returns("70");

            // initialise repository
            _libraryService = new LibraryService(context, _mapper, _config.Object);
        }

        [Fact]
        public async Task Search_ValidPublicationYear()
        {
            // Arrange
            var searchTerm = "2000";
            var expectedPublicationYear = 2000;

            // Act
            var result = await _libraryService.Search(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.All(book => book.PublicationYear == expectedPublicationYear));
        }

        [Fact]
        public async Task Search_InvalidPublicationYear()
        {
            // Arrange
            var searchTerm = "100000";

            // Act
            var result = await _libraryService.Search(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Search_ValidFuzzyTypeSearchTerm()
        {
            // Arrange
            var searchTerm = "NonFictional";
            var expectedBookType = "Non-Fictional";

            // Act
            var result = await _libraryService.Search(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.All(book => book.Type == expectedBookType));
        }

        [Fact]
        public async Task Search_WithEmptySearchTerm()
        {
            // Arrange
            var searchTerm = string.Empty;

            // Act
            var result = await _libraryService.Search(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Search_WithNonExistingSearchTerm()
        {
            // Arrange
            var searchTerm = "NonExistingTerm";

            // Act
            var result = await _libraryService.Search(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

    }
}