using Microsoft.EntityFrameworkCore;

namespace Task.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<Objek> obj { get; set; } //setiap nambah model harus nambah context
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}