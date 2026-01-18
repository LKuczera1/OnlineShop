using Azure;
using Catalog;
using Catalog.Models;
using Catalog.Services;
using Identity.Dtos;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Enums;
using Xunit;

namespace UnitTests.UnitTests.Services
{
    public class IdentityServiceTests : ServicesTestsBase
    {
        private readonly IdentityDbContext _db;
        private readonly Identity.Services.IdentityServices _service;

        private readonly static int numberOfTestAccounts = 4;

        public IdentityServiceTests()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase($"ProductTestDB_{Guid.NewGuid()}")
            .Options;

            var config = new ConfigurationBuilder()
            .AddJsonFile(Utility.Common.Tools.GetAppSettingsDirectory(), optional: true)
            .Build();

            Console.WriteLine(Utility.Common.Tools.GetAppSettingsDirectory());

            _db = new IdentityDbContext(options);

            var jwtService = new JWTService(config);
            var identityService = new IdentityServices(_db, jwtService, config);

            _service = new IdentityServices(_db, jwtService, config);

            var accounts = JsonSerializer.Deserialize<List<Account>>(loadDbSource("IdentityDb.json"));

            _db.UserAccounts.AddRange(accounts!);
            _db.SaveChanges();
        }

        [Fact]
        public async Task ShouldReceiveListOfAccounts()
        {
            var list = await _service.GetAccounts();

            Assert.Equal(numberOfTestAccounts, list.Count());
        }

        [Theory]
        [InlineData("TestAccount1", "12345", typeof(OkObjectResult))] //Valid login data
        [InlineData("NotExistingAccount", "12345", typeof(NotFoundObjectResult))] //Invalid login(username)
        [InlineData("TestAccount1", "1234222225", typeof(BadRequestObjectResult))] //Invalid Password
        public async Task LoginTest(string login, string password, Type expectedResult)
        {
            var request = new LoginRequestDto()
            {
                UserName = login,
                Password = password
            };

            var response = await _service.Login(request);

            Assert.IsType(expectedResult, response);
        }

        [Theory]
        [InlineData("", "", "", "", "", typeof(BadRequestObjectResult))] //Empty form
        [InlineData("TestAccount1", "222", "222", "222", "222", typeof(BadRequestObjectResult))] //Existing username
        [InlineData("NewAccount", "Password", "Randomemail", "Address", "City", typeof(OkObjectResult))] //Valid form
        public async Task RegisterTest(string userName, string password, string email, string address, string city, Type expectedResult)
        {
            var registerRequest = new AccountDto()
            {
                UserName = userName,
                Password = password,
                Email = email,
                PhoneNumber = null,
                Address = address,
                City = city,
            };

            var response = await _service.Register(registerRequest);

            Assert.IsType(expectedResult, response);

            if (response is OkObjectResult)
            {
                var result = await _service.GetAccounts();
                int accountsCount = result.Count();

                Assert.True(accountsCount > numberOfTestAccounts);
            }
        }

        //---------------------------------------------
        // 3 tests for .GetAccountById(...), because there are more than 4 possible results

        [Theory]
        [InlineData(PriviledgeLevel.Admin)]
        [InlineData(PriviledgeLevel.Customer)]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker)]
        [InlineData(PriviledgeLevel.NotAssigned)]
        public async Task GetAccountById_NotFound(PriviledgeLevel role)
        {
            var res = await _service.GetAccountById(9999, new UserData(0, role));
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, true)]
        [InlineData(PriviledgeLevel.Customer, true)]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, true)]
        [InlineData(PriviledgeLevel.NotAssigned, false)]
        public async Task GetAccountById_Self(PriviledgeLevel role, bool shouldBeOk)
        {
            int myId = 1;

            var res = await _service.GetAccountById(myId, new UserData(myId, role));

            if (shouldBeOk)
                Assert.True(res.Result is OkObjectResult || res.Value != null); // implicit 200
            else
                Assert.IsType<ForbidResult>(res.Result);
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, true)]
        [InlineData(PriviledgeLevel.Customer, false)]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, false)]
        [InlineData(PriviledgeLevel.NotAssigned, false)]
        public async Task GetAccountById_Other(PriviledgeLevel role, bool shouldBeOk)
        {
            int targetId = 1;     // istnieje
            int myId = 2;         // inny użytkownik

            var res = await _service.GetAccountById(targetId, new UserData(myId, role));

            if (shouldBeOk)
                Assert.True(res.Result is OkObjectResult || res.Value != null); // implicit 200
            else
                Assert.IsType<ForbidResult>(res.Result);
        }

        //---------------------------------------------

        [Theory]
        [InlineData(Utility.Enums.PriviledgeLevel.Admin)]
        [InlineData(Utility.Enums.PriviledgeLevel.Customer)]
        [InlineData(Utility.Enums.PriviledgeLevel.SalesDepartmentWorker)]
        [InlineData(Utility.Enums.PriviledgeLevel.NotAssigned)]
        public async Task PutAccountTest(PriviledgeLevel privilegeLevel)
        {
            int accountId = 1;

            var account = await _service.GetAccountById(accountId, new UserData(accountId, PriviledgeLevel.Admin));

            account.Value.UserName = privilegeLevel.ToString();

            var response = await _service.PutAccount(accountId, account.Value, new UserData(0, privilegeLevel));

            //Test - Only admin should be able to change account data with differend id
            if (privilegeLevel == PriviledgeLevel.Admin)
            {
                Assert.IsType<NoContentResult>(response);
            }
            else
            {
                Assert.IsType<ForbidResult>(response);
            }

            if (privilegeLevel == PriviledgeLevel.NotAssigned) return;

            await _service.PutAccount(accountId, account.Value, new UserData(accountId, privilegeLevel));

            var changedAccount = await _service.GetAccountById(accountId, new UserData(accountId, PriviledgeLevel.Admin));

            Assert.True(changedAccount.Value.UserName == account.Value.UserName);

        }


        [Theory]
        [InlineData(Utility.Enums.PriviledgeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PostAccountTest(PriviledgeLevel privilegeLevel, Type expectedResponse)
        {
            int numberOfAccounts = await GetNumberOfAccounts();

            var account = new AccountDto()
            {
                UserName = "Test" + privilegeLevel.ToString(),
                Password = "password",
                PhoneNumber = "1234567890",
                PrivilegeLevel = PriviledgeLevel.NotAssigned,
                City = "City",
                Address = "Address",
                Email = "Email"
            };

            var result = await _service.PostAccount(account, new UserData(0, privilegeLevel));

            Assert.IsType(expectedResponse, result.Result);

            int numberOfAccountsCheck = await GetNumberOfAccounts();

            if (result.Result is CreatedAtRouteResult created)
            {
                var idObj = created.RouteValues?["id"] ?? created.RouteValues?["Id"];
                Assert.NotNull(idObj);

                int newId = Convert.ToInt32(idObj);

                Assert.Equal(numberOfAccounts + 1, numberOfAccountsCheck);

                var get = await _service.GetAccountById(newId, new UserData(0, PriviledgeLevel.Admin));
                Assert.True(get.Result is OkObjectResult || get.Value != null);
                Assert.Equal(account.UserName, get.Value!.UserName);
            }
            else
            {
                Assert.Equal(numberOfAccounts, numberOfAccountsCheck);
            }
        }

        [Theory]
        [InlineData(Utility.Enums.PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task DeleteAccountTest(PriviledgeLevel privilegeLevel, Type expectedResponse)
        {
            if (privilegeLevel == PriviledgeLevel.Admin)
            {
                //Test for not existing account

                var nonExistincAccountResult = await _service.DeleteAccount(await GetNumberOfAccounts() + 100, new UserData(0, privilegeLevel));

                Assert.IsType<NotFoundResult>(nonExistincAccountResult);
            }

            int numberOfAccounts = await GetNumberOfAccounts();

            var result = await _service.DeleteAccount(await GetNumberOfAccounts(), new UserData(0, privilegeLevel));

            Assert.IsType(expectedResponse, result);

            int numberOfAccountsCheck = await GetNumberOfAccounts();

            if (privilegeLevel == PriviledgeLevel.Admin)
            {
                Assert.True(numberOfAccounts > numberOfAccountsCheck);
            }
            else
            {
                Assert.True(numberOfAccounts == numberOfAccountsCheck);
            }
        }

        [Theory]
        [InlineData(Utility.Enums.PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(Utility.Enums.PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task SetPrivilegeLevelTest(PriviledgeLevel privilegeLevel, Type expectedResponse)
        {
            if (privilegeLevel == PriviledgeLevel.Admin)
            {
                //Test for not existing account

                var nonExistincAccountResult = await _service.SetPrivilegeLevel(await GetNumberOfAccounts() + 100, PriviledgeLevel.NotAssigned, new UserData(0, privilegeLevel));

                Assert.IsType<NotFoundResult>(nonExistincAccountResult);
            }

            int accountID = 1;

            var temp1 = await _service.GetAccountById(accountID, new UserData(0, PriviledgeLevel.Admin));

            var result = await _service.SetPrivilegeLevel(accountID, PriviledgeLevel.NotAssigned, new UserData(0, privilegeLevel));

            var temp2 = await _service.GetAccountById(accountID, new UserData(0, PriviledgeLevel.Admin));

            if (privilegeLevel == PriviledgeLevel.Admin)
            {
                Assert.IsType(expectedResponse, result);
                Assert.True(temp1.Value.PrivilegeLevel != temp2.Value.PrivilegeLevel);
            }
            else
            {
                Assert.IsType(expectedResponse, result);
                Assert.True(temp1.Value.PrivilegeLevel == temp2.Value.PrivilegeLevel);
            }
        }

        public async Task<int> GetNumberOfAccounts()
        {
            var result = await _service.GetAccounts();

            if (result is null) return 0;

            return result.Count();
        }
    }
}

