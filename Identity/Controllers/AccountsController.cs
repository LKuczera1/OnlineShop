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
using Utility.Common;
using Utility.Enums;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : CustomControllerBase
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
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker_Customer)]
        public async Task<ActionResult<AccountDto>> GetAccount(int id)
        {
            return await _context.GetAccountById(id, GetUserData());
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker_Customer)]
        public async Task<IActionResult> PutAccount(int id, AccountDto account)
        {
            return await _context.PutAccount(id, account, GetUserData());
        }

        // PUT: api/Accounts/5
        [HttpPut("setPrivilegeLevel/{id}/{privilegeLevel}")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IActionResult> SetPrivilegeLevel(int id, PriviledgeLevel privilegeLevel)
        {
            return await _context.SetPrivilegeLevel(id, privilegeLevel, GetUserData());
        }


        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<ActionResult<AccountDto>> PostAccount(AccountDto account)
        {
            return await _context.PostAccount(account, GetUserData());
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

        [HttpPost("login/refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh(RefreshRequestDto request)
        {
            return await _context.Refresh(request, GetUserData());
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            return await _context.DeleteAccount(id, GetUserData());
        }

        /*
        private bool AccountExists(int id)
        {
            return _context.UserAccounts.Any(e => e.Id == id);
            
            
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //
            // _context.Save....().AsNoTracking() <- Poczytać bo podobno bardzo przyśpiesza odczyt z bazy
        }
        */
    }
}

