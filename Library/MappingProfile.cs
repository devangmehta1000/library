using AutoMapper;

namespace Library;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Data.Models.Book, Services.Models.Book>();
        CreateMap<Data.Models.Author, Services.Models.Author>();

        // fetch the authorname property by concatenating firstaname and lastname
        CreateMap<Services.Models.Book, Controllers.Models.Book>()
             .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));

        
        CreateMap<Services.Models.Book, Controllers.Models.Author>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
            .ForMember(dest => dest.DateOfBirth, opt =>
            {
                // instead of specifying the outpput dob as string, the similar result could be achived by using json formatters in program startup.
                // Though this would be done depending on the requirements of the project as we would not want to strip the time component of all datetime properties
                // Hence we would then require some other mechanism to conditionally format only certain datetime properties to date
                opt.PreCondition(src => src.Author.DateOfBirth.HasValue);
                opt.MapFrom(src => src.Author.DateOfBirth.Value.ToString("yyyy-MM-dd"));
            })
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Author.Gender))

            // using custom resolvers in automapper to calculate the age and set theAuthoredBooks properties on Author
            .ForMember(dest => dest.BooksAuthored, opt => opt.MapFrom<BookNameResolver>())
            .ForMember(dest => dest.Age, opt => opt.MapFrom<AgeResolver>()); 
    }
}