using Microsoft.EntityFrameworkCore;

namespace Shopping
{
    public class ShoppingDbContext :DbContext
    {
        public DbSet<String> Wishlist { get; set; }
        public DbSet<String> ShoppingCart { get; set; }

        public DbSet<String> Orders { get; set; } // I status zamówienia


        public ShoppingDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
