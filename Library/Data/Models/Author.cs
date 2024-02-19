using System.ComponentModel.DataAnnotations;

namespace Library.Data.Models;
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
    public DateTime? DateOfBirth { get; set; }
    public List<Book> Books { get; set; }
}