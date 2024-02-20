using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FuzzySharp;
using Library.Data;

namespace Library.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly int FuzzyTolerance;
        private readonly LibraryContext context;
        private readonly IMapper mapper;

        public LibraryService(LibraryContext context, IMapper mapper, IConfiguration config)
        {
            this.context = context;
            this.mapper = mapper;
            this.FuzzyTolerance =  int.Parse(config["Fuzzy:Tolerance"]);
        }

        /// <summary>
        /// Function to search books and authors using the search term provided.
        /// The function looks for a match in the following fields -  Book.PublicationYear, Book.Title, Book.Genre, Book.Type, Author.FirstName, Author.LastName
        /// Fuzzy search matches the search term to the fields depending on the match ratio - FuzzySharp is used to achieve this - https://github.com/JakeBayer/FuzzySharp
        /// </summary>
        /// <param name="searchTerm">Search parameter</param>
        /// <returns>List of matched books and authors</returns>

        public async Task<List<Services.Models.Book>> Search(string searchTerm)
        {
            var searchedBooks = new List<Data.Models.Book>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var books = await context.Books.Include(b => b.Author).ToListAsync();

                searchedBooks = books.Where(book =>
                        IsStringMatch(book.PublicationYear?.ToString(), searchTerm) || // Convert to string and check for partial years
                        IsStringMatch(book.Title, searchTerm) ||
                        IsFuzzyMatch(book.Title, searchTerm, FuzzyTolerance) ||
                        IsStringMatch(book.Genre, searchTerm) ||
                        IsFuzzyMatch(book.Genre, searchTerm, FuzzyTolerance) ||
                        IsStringMatch(book.Type, searchTerm) ||
                        IsFuzzyMatch(book.Type, searchTerm, FuzzyTolerance) ||
                        IsStringMatch(book.Author.FirstName, searchTerm) ||
                        IsFuzzyMatch(book.Author.FirstName, searchTerm, FuzzyTolerance) ||
                        IsStringMatch(book.Author.LastName, searchTerm) ||
                        IsFuzzyMatch(book.Author.LastName, searchTerm, FuzzyTolerance))
                    .ToList();
            }

            return mapper.Map<List<Services.Models.Book>>(searchedBooks);
        }

        // checks if a string is null or contains the search term
        private static bool IsStringMatch(string value, string searchTerm)
        {
            return value != null && value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        // checks if a fuzzy match ratio is above a certain tolerance
        private static bool IsFuzzyMatch(string value, string searchTerm, int tolerance)
        {
            return value != null && Fuzz.PartialRatio(value, searchTerm) >= tolerance;
        }
    }
}