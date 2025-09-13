using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalog;
using Identity.Models;
using Identity.Services;
using Identity.Dtos;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IdentityServices _context;

        public AccountsController(IdentityServices context)
        {
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<IEnumerable<AccountDto>> GetUserAccounts()
        {
            return await _context.GetAccounts();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDto>> GetAccount(int id)
        {
            return await _context.GetAccountById(id);
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, AccountDto account)
        {
            return await _context.PutAccount(id, account);
        }

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AccountDto>> PostAccount(AccountDto account)
        {
            return await _context.PostAccount(account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            return await _context.DeleteAccount(id);
        }

        /*
        private bool AccountExists(int id)
        {
            return _context.UserAccounts.Any(e => e.Id == id);
        }
        */
    }
}
