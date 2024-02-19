using System.ComponentModel.DataAnnotations;

namespace Library.Services.Models;
public class Author
{
    [Required]
    public int AuthorId { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public required string FirstName { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public required string LastName { get; set; }
    [Required]
    public required string Gender { get; set; }
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    public List<Book> Books { get; set; }
}