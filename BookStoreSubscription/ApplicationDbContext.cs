using BookStoreSubscription.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStoreSubscription;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuthorBook>()
            .HasKey(al => new { al.AuthorId, al.BookId });

    }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<AuthorBook> AuthorBooks { get; set; }
    public DbSet<KeyAPI> KeyAPIs { get; set; }
    public DbSet<Petition> Petitions { get; set; }
    public DbSet<DomainRestriction> DomainRestrictions { get; set; }
    public DbSet<IpRestriction> IpRestrictions { get; set; }
}

