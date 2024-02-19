using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FuzzySharp;
using Library.Data;

namespace Library.Services
{
    public class LibraryService(LibraryContext context, IMapper mapper) : ILibraryService
    {
        private const int FuzzyTolerance = 70;
        private const int MinPublicationYear = 1500;

        public async Task<List<Services.Models.Book>> Search(string searchTerm)
        {
            var searchedBooks = new List<Data.Models.Book>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var books = await context.Books.Include(b => b.Author).ToListAsync();

                searchedBooks = books.Where(book =>
                    (book.PublicationYear != null && book.PublicationYear.ToString().Contains(searchTerm)) || // books will match search criteria for partial years e.g. 84 ill match 1984
                    book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    Fuzz.PartialRatio(book.Title, searchTerm) >= FuzzyTolerance ||
                    book.Genre != null && (book.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    Fuzz.PartialRatio(book.Genre, searchTerm) >= FuzzyTolerance) ||
                    book.Type != null && (book.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    Fuzz.PartialRatio(book.Type, searchTerm) >= FuzzyTolerance) ||
                    book.Author.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    Fuzz.PartialRatio(book.Author.FirstName, searchTerm) >= FuzzyTolerance ||
                    book.Author.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    Fuzz.PartialRatio(book.Author.LastName, searchTerm) >= FuzzyTolerance
                ).ToList();                
            }

            return mapper.Map<List<Services.Models.Book>>(searchedBooks);
        }
    }
}