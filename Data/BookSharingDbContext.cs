using Booksharing.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookSharing.Data
{
    public class BookSharingDbContext:DbContext
    {
        public DbSet<BookModel> Books { get; set; }

        public BookSharingDbContext(DbContextOptions<BookSharingDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}