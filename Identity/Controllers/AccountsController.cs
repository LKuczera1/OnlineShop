using Catalog;
using Identity.Dtos;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility.Enums;

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
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IEnumerable<AccountDto>> GetUserAccounts()
        {
            return await _context.GetAccounts();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}", Name = "GetAccountById")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<ActionResult<AccountDto>> GetAccount(int id)
        {
            return await _context.GetAccountById(id);
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IActionResult> PutAccount(int id, AccountDto account)
        {
            return await _context.PutAccount(id, account);
        }

        // PUT: api/Accounts/5
        [HttpPut("setPriviledgeLevel/{id}/{priviledgeLevel}")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IActionResult> SetPriviledgeLevel(int id, PriviledgeLevel priviledgeLevel)
        {
            return await _context.SetPriviledgeLevel(id, priviledgeLevel);
        }


        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<ActionResult<AccountDto>> PostAccount(AccountDto account)
        {
            return await _context.PostAccount(account);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            return await _context.Login(loginRequest);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AccountDto account)
        {
            return await _context.Register(account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RolesStr.Admin)]
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
