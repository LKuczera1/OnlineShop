using Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog
{
    public class IdentityDbContext :DbContext
    {
        public DbSet<Product> Products { get; set; }
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
    }
}
