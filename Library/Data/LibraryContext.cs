using Microsoft.EntityFrameworkCore;

namespace Library.Data;
public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<Models.Book> Books { get; set; }
    public DbSet<Models.Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Author can have multiple Books, but books needs to have only one author
        modelBuilder.Entity<Models.Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}