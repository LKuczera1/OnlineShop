using Catalog;
using Identity.Dtos;
using Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class IdentityServices
    {

        private readonly IdentityDbContext _context;

        public IdentityServices(IdentityDbContext context)
        {
            _context = context;
        }

        //Get
        public async Task<IEnumerable<AccountDto>> GetAccounts()
        {
            var accountsList = await _context.Set<Account>().ToListAsync();

            var account = accountsList.Select(p => p.ToDto());

            return account;
        }

        //Get by Id
        public async Task<AccountDto> GetAccountById(int id)
        {
            var account = await _context.Set<Account>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            return account.ToDto();
        }

        //Put
        public async Task<IActionResult> PutAccount(int id, AccountDto dto)
        {
            var entity = await _context.Set<Account>().FindAsync([id]);
            if (entity is null)
                return new NotFoundResult();

            entity = dto.ToEntity(id);

            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<AccountDto>> PostAccount(AccountDto dto)
        {
            var entity = dto.ToEntity(0);

            _context.Set<Account>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(PostAccount), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.UserAccounts.FindAsync(id);
            if (account == null)
            {
                return new NotFoundResult();
            }

            _context.UserAccounts.Remove(account);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
