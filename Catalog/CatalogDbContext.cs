using Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog
{
    public class CatalogDbContext :DbContext
    {
        public DbSet<Product> Products { get; set; }
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
    }
}
