using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping
{
    public class ShoppingDbContext :DbContext
    {
        public DbSet<WishlistItem> Wishlist { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedItem> OrderedItems { get; set; }


        public ShoppingDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
