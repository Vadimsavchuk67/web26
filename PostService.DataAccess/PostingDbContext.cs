using Microsoft.EntityFrameworkCore;
using PostService.Models;

namespace PostService.DataAccess;

public class PostingDbContext : DbContext
{
    public DbSet<Posting> Postings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Posting>()
            .Property(p => p.CreatedAt)
            .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "PostingEfDb.db");
        optionsBuilder.UseSqlite($"Data Source={path}");
    }
}
