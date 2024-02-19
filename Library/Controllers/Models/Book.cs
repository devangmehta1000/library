using System.ComponentModel.DataAnnotations;

namespace Library.Controllers.Models;
public class Book
{
    [Required]
    public int BookId { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(255)]
    public required string Title { get; set; }
    [Required]
    public int PublicationYear { get; set; }
    [MaxLength(50)]
    public string? Type { get; set; }
    [MaxLength(50)]
    public string? Genre { get; set; }
    [Required]
    public string AuthorName { get; set; }
}