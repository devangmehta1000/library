using AutoMapper;
namespace Library;
public class BookNameResolver : IValueResolver<Services.Models.Book, Controllers.Models.Author, List<string>>
{
    public List<string> Resolve(Services.Models.Book source, Controllers.Models.Author destination, List<string> destMember, ResolutionContext context)
    {        
        return source.Author.Books.Select(b => b.Title).ToList();
    }
}