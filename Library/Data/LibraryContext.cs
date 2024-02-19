using Microsoft.EntityFrameworkCore;

namespace Library.Data;
public class LibraryContext : DbContext
{
    public DbSet<Models.Book> Books { get; set; }
    public DbSet<Models.Author> Authors { get; set; }

    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Author can have multiple Books. Books needs to have one author
        modelBuilder.Entity<Models.Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Specify additional constraints or configurations if needed
        // For example, you may want to configure the maximum length of a string property:
        //modelBuilder.Entity<Book>().Property(b => b.Title).HasMaxLength(255).;
    }
}