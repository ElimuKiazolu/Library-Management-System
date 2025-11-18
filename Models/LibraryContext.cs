using System.Data.Entity;

namespace MiniLibrary.Models
{
    public class LibraryContext : DbContext
    {
        public LibraryContext() : base("DefaultConnection") { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
