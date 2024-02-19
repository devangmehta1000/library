using System.ComponentModel.DataAnnotations;

namespace Library.Services.Models;
public class Book
{
    [Required]
    public int BookId { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(255)]
    public required string Title { get; set; }
    public int? PublicationYear { get; set; }
    [MaxLength(50)]
    public string? Type { get; set; }
    [MaxLength(50)]
    public string? Genre { get; set; }
    [Required]
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}