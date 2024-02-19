
namespace Library.Services
{
    public interface ILibraryService
    {
        public Task<List<Models.Book>> Search(string searchTerm);

    }
}