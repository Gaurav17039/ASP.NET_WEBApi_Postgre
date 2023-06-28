using Microsoft.EntityFrameworkCore;

namespace CRUD_Migration_Logging_XunitTesting.Models
{
    public class ShoppingContext : DbContext
    {
        public ShoppingContext(DbContextOptions<ShoppingContext> options) : base(options)
        {
        }

        public DbSet<ShoppingItem> ShoppingItems { get; set; }
    }
}
