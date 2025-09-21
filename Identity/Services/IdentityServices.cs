using Catalog;
using Identity.Dtos;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility.Common;
using Utility.DtoEntity;
using Utility.Enums;

namespace Identity.Services
{
    public class IdentityServices
    {

        private readonly IdentityDbContext _context;
        private readonly JWTService _jwtService;
        private readonly IConfiguration _config;

        //Haszowanie hasła
        private readonly PasswordHasher<AccountDto> _hasher = new();

        public IdentityServices(IdentityDbContext context, JWTService jwtService, IConfiguration config)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;
        }

        //Get
        public async Task<IEnumerable<AccountDto>> GetAccounts()
        {
            var accountsList = await _context.Set<Account>().ToListAsync();

            var account = accountsList.Select(p => p.ToDto());

            return account;
        }

        //Get by Id
        public async Task<ActionResult<AccountDto>> GetAccountById(int id, UserData userData)
        {
            var account = await _context.Set<Account>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            if (account == null)
            {
                return new NotFoundResult();
            }

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                    return account.ToDto();
                    break;
                case PrivilegeLevel.SalesDepartmentWorker:
                case PrivilegeLevel.Customer:
                    if (account.Id != userData.clientId) return new ForbidResult();

                    return account.ToDto();

                    break;
                default: return new ForbidResult();
            }
        }

        //Put
        public async Task<IActionResult> PutAccount(int id, AccountDto dto, UserData userData)
        {
            var entity = await _context.Set<Account>().FindAsync(id);
            if (entity is null)
                return new NotFoundResult(); 
            
            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                    entity.FromDto(id, dto);
                    break;
                case PrivilegeLevel.SalesDepartmentWorker:
                case PrivilegeLevel.Customer:
                    if (entity.Id != userData.clientId) return new ForbidResult();

                    entity.FromDto(id, dto);

                    break;
                default: return new ForbidResult();
            }

            entity.FromDto(id, dto);

            //_context.Entry(entity).State = EntityState.Modified;
            //Zakomentowane aby API nie zwracalo bledu 500

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<AccountDto>> PostAccount(AccountDto dto, UserData userData)
        {
            var entity = dto.ToEntity(0);

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                    _context.Set<Account>().Add(entity);
                    await _context.SaveChangesAsync();
                    break;
                default: return new ForbidResult();
            }

            return new CreatedAtRouteResult(nameof(GetAccountById), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteAccount(int id, UserData userData)
        {
            var account = await _context.UserAccounts.FindAsync(id);
            if (account == null)
            {
                return new NotFoundResult();
            }

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                    _context.UserAccounts.Remove(account);
                    await _context.SaveChangesAsync();
                    break;
                default: return new ForbidResult();
            }

            return new NoContentResult();
        }

        public async Task<IActionResult> SetPriviledgeLevel(int id, PrivilegeLevel priviledgeLevel, UserData userData)
        {
            var entity = await _context.Set<Account>().FindAsync([id]);
            if (entity is null)
                return new NotFoundResult(); 
            
            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                    entity.PriviledgeLevel = priviledgeLevel;

                    await _context.SaveChangesAsync();
                    break;
                default: return new ForbidResult();
            }

            
            return new NoContentResult();
        }

        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var entity = await _context.UserAccounts.FirstOrDefaultAsync(user => user.UserName == loginRequest.UserName);

            if (entity == null) return new NotFoundObjectResult("Account with such username was not found");

            if(VerifyPassword(loginRequest, entity.ToDto()) == PasswordVerificationResult.Success)
            {
                var JWTtoken = _jwtService.GenerateToken(entity.Id, entity.UserName, entity.PriviledgeLevel.ToString());

                return new OkObjectResult(new AuthResponseDto
                {
                    Token = JWTtoken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"]!)),
                    UserId = entity.Id,
                    UserName = entity.UserName,
                    PriviledgeLevel = entity.PriviledgeLevel
                });
            }
            else
            {
                return new BadRequestObjectResult("Wrong password");
            }
        }

        public async Task<IActionResult> Register(AccountDto account)
        {
            if (string.IsNullOrWhiteSpace(account.UserName) ||
                string.IsNullOrWhiteSpace(account.Password) ||
                string.IsNullOrWhiteSpace(account.Email) ||
                string.IsNullOrWhiteSpace(account.Address) ||
                string.IsNullOrWhiteSpace(account.City))
            {
                return new BadRequestObjectResult("The registration form was not completed correctly");
            }

            var entity = await _context.UserAccounts.FirstOrDefaultAsync(user => 
                user.UserName.ToLower() == account.UserName.ToLower());

            if(entity != null)
            {
                return new BadRequestObjectResult("Account with this username already exists.");
            }

            account.PriviledgeLevel = PrivilegeLevel.Customer;

            account.Password = HashPassword(account);

            _context.Set<Account>().Add(account.ToEntity(0));
            await _context.SaveChangesAsync();

            return new OkObjectResult("Account was succesfully created");
        }

        private string HashPassword(AccountDto account)
        {
            return _hasher.HashPassword(account, account.Password);
        }

        private PasswordVerificationResult VerifyPassword(LoginRequestDto request, AccountDto user)
        {
            return _hasher.VerifyHashedPassword(user, user.Password, request.Password);
        }
    }
}
