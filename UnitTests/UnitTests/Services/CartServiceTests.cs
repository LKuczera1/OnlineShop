using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping;
using Shopping.Dtos;
using Shopping.Models;
using Shopping.Services;
using System.Text.Json;
using Utility.Common;
using Utility.Enums;
using Xunit;

namespace UnitTests.UnitTests.Services
{
    public class CartServiceTests : ServicesTestsBase
    {
        private readonly ShoppingDbContext _db;
        private readonly CartService _service;

        public CartServiceTests()
        {
            var options = new DbContextOptionsBuilder<ShoppingDbContext>()
                .UseInMemoryDatabase($"ShoppingCartTestDB_{Guid.NewGuid()}")
                .Options;

            _db = new ShoppingDbContext(options);
            _service = new CartService(_db);

            var json = loadDbSource("ShoppingCartItemDb.json");
            var items = JsonSerializer.Deserialize<List<ShoppingCartItem>>(json!) ?? new();
            _db.Set<ShoppingCartItem>().AddRange(items);
            _db.SaveChanges();
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, 10)]
        [InlineData(PriviledgeLevel.Customer, 4)]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, -1)] // Forbid
        [InlineData(PriviledgeLevel.NotAssigned, -1)] // Forbid
        public async Task GetShoppingCartItems_Test(PriviledgeLevel privilege, int expectedCountOrForbid)
        {
            var ud = new UserData(1, privilege);
            var res = await _service.GetShoppingCartItems(ud);

            if (expectedCountOrForbid >= 0)
            {
                Assert.True(res.Result is OkObjectResult || res.Value != null);
                var list = (res.Result as OkObjectResult)?.Value as IEnumerable<ShoppingCartItemDto> ?? res.Value!;
                if (privilege == PriviledgeLevel.Admin)
                    Assert.Equal(expectedCountOrForbid, list.Count());
                else
                    Assert.All(list, i => Assert.Equal(1, i.ClientId));
            }
            else
            {
                Assert.IsType<ForbidResult>(res.Result);
            }
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, 0, 1, true)]
        [InlineData(PriviledgeLevel.Customer, 2, 1, true)]   // Customer - Owner
        [InlineData(PriviledgeLevel.Customer, 2, 2, false)]  // Customer - Not Owner
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, 0, 1, false)]
        [InlineData(PriviledgeLevel.NotAssigned, 0, 1, false)]
        public async Task GetShoppingCartItemById_Permissions(PriviledgeLevel privilege, int clientId, int itemId, bool shouldBeOk)
        {
            var result = await _service.GetShoppingCartItemById(itemId, new UserData(clientId, privilege));

            if (shouldBeOk)
            {
                Assert.True(result.Result is OkObjectResult || result.Value != null);
            }
            else
            {
                Assert.True(
                    result.Result is ForbidResult ||
                    result.Result is NotFoundResult
                );
            }
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(CreatedAtRouteResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PostShoppingCartItem_Test(PriviledgeLevel privilege, Type expected)
        {
            var dto = new ShoppingCartItemDto
            {
                ClientId = 999,
                ProductId = 999,
                Quantity = 2,
                Price = 19.99
            };

            var res = await _service.PostShoppingCartItem(dto, new UserData(1, privilege));
            Assert.IsType(expected, res.Result);
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PutShoppingCartItem_Test(PriviledgeLevel privilege, Type expectedType)
        {
            int targetId = 4;
            var dto = new ShoppingCartItemDto { ClientId = 999, ProductId = 777, Quantity = 5, Price = 10.5 };

            var res = await _service.PutShoppingCartItem(targetId, dto, new UserData(2, privilege));

            if (expectedType == typeof(NoContentResult))
                Assert.IsType<NoContentResult>(res);
            else
                Assert.IsType(expectedType, res);
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task DeleteShoppingCartItem_Test(PriviledgeLevel privilege, Type expected)
        {
            var result = await _service.DeleteShoppingCartItem(4, new UserData(2, privilege));
            if (expected == typeof(NoContentResult))
                Assert.IsType<NoContentResult>(result);
            else
                Assert.IsType(expected, result);
        }

        [Fact]
        public async Task DeleteShoppingCartItemsByClientId_Admin_NoContent()
        {
            var before = await _service.GetShoppingCartItemByClientId(1, new UserData(1, PriviledgeLevel.Admin));
            var result = await _service.DeleteShoppingCartItemsByClientId(1, new UserData(0, PriviledgeLevel.Admin));

            Assert.IsType<NoContentResult>(result);

            var after = await _service.GetShoppingCartItemByClientId(1, new UserData(1, PriviledgeLevel.Admin));

            var beforeCount = (before.Result as OkObjectResult)?.Value as List<ShoppingCartItemDto> ?? before.Value!;
            var afterCount = (after.Result as OkObjectResult)?.Value as List<ShoppingCartItemDto> ?? after.Value!;

            Assert.True(afterCount.Count <= Math.Max(0, beforeCount.Count - 1));
        }
    }
}
