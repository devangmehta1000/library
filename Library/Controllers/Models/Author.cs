using System.ComponentModel.DataAnnotations;

namespace Library.Controllers.Models;
public class Author
{
    [Required]
    public int AuthorId { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(250)]
    public required string FullName { get; set; }
    [Required]
    public required string Gender { get; set; }
    public string? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public List<string> BooksAuthored { get; set; } 
    
}