using AutoMapper;

namespace Library;
public class AgeResolver : IValueResolver<Services.Models.Book, Controllers.Models.Author, int?>
{
    public int? Resolve(Services.Models.Book source, Controllers.Models.Author destination, int? destMember, ResolutionContext context)
    {
        if (source.Author.DateOfBirth == null)
        {
            return null;
        }

        DateTime currentDate = DateTime.Now;
        int age = currentDate.Year - source.Author.DateOfBirth.Value.Year;

        // Check if the birthday has occurred this year
        if (currentDate.Month < source.Author.DateOfBirth.Value.Month || 
            (currentDate.Month == source.Author.DateOfBirth.Value.Month && currentDate.Day < source.Author.DateOfBirth.Value.Day))
        {
            age--;
        }

        return age;
    }
}