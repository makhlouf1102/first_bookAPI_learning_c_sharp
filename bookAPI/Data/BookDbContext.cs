using bookAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace bookAPI.Data
{
    // class that inherates from DbContext
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options)
            :base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
