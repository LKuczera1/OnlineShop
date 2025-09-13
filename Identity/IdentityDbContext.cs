using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog
{
    public class IdentityDbContext :DbContext
    {
        public DbSet<Account> UserAccounts { get; set; }
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }
    }
}
