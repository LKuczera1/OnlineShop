using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;
using Shopping.Services;
using System.Text.Json;
using Utility.Common;
using Utility.Enums;
using Xunit;
using System.Linq;
using Shopping;

namespace UnitTests.UnitTests.Services
{
    public class WishlistServiceTests : ServicesTestsBase
    {
        private readonly ShoppingDbContext _db;
        private readonly WishlistService _service;

        public WishlistServiceTests()
        {
            var options = new DbContextOptionsBuilder<ShoppingDbContext>()
                .UseInMemoryDatabase($"WishlistTestDB_{Guid.NewGuid()}")
                .Options;

            _db = new ShoppingDbContext(options);
            _service = new WishlistService(_db);

            var json = loadDbSource("WishlistItemDb.json");
            var items = JsonSerializer.Deserialize<List<WishlistItem>>(json!) ?? new();
            _db.Set<WishlistItem>().AddRange(items);
            _db.SaveChanges();
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, 10)]
        [InlineData(PrivilegeLevel.Customer, -1)]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, -2)]
        [InlineData(PrivilegeLevel.NotAssigned, -2)]
        public async Task GetWishlistItems_Test(PrivilegeLevel privilege, int expectedCountOrFlag)
        {
            var result = await _service.GetWishlistItems(new UserData(1, privilege));

            if (expectedCountOrFlag == 10)
            {
                Assert.NotNull(result);
                Assert.Equal(10, result.Count());
            }
            else if (expectedCountOrFlag == -1)
            {
                Assert.NotNull(result);
                Assert.All(result, w => Assert.Equal(1, w.ClientId));
            }
            else
            {
                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(OkObjectResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(UnauthorizedResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(UnauthorizedResult))]
        public async Task GetWishlistItemById_Basic_ByPrivilege(PrivilegeLevel privilege, Type expected)
        {
            var id = _db.Set<WishlistItem>().Select(w => w.Id).First();
            var result = await _service.GetWishlistItemById(id, new UserData(1, privilege));

            if (result.Result is null)
            {
                //In case of succesfull verification service returns only dto, result.Result = null
                Assert.True(result != null && result.Value != null);
            }
            else
            {
                Assert.IsType(expected, result.Result);
            }
        }

        [Fact]
        public async Task GetWishlistItemById_Admin_NotFound()
        {
            var result = await _service.GetWishlistItemById(9999, new UserData(1, PrivilegeLevel.Admin));
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetWishlistItemById_Customer_OwnVsForeign()
        {
            var itemId = _db.Set<WishlistItem>().Select(w => w.Id).First();
            var item = await _service.GetWishlistItemById(itemId, new UserData(0, PrivilegeLevel.Admin));

            Assert.NotNull(item.Value);

            var ownerId = item.Value.ClientId;
            var otherId = ownerId + 1;

            //test for item with user id
            var own = await _service.GetWishlistItemById(itemId, new UserData(ownerId, PrivilegeLevel.Customer));
            Assert.True(own != null && own.Value != null);

            //test for foreign id
            var foreign = await _service.GetWishlistItemById(itemId, new UserData(otherId, PrivilegeLevel.Customer));
            Assert.IsType<NotFoundResult>(foreign.Result);
        }


        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(CreatedAtRouteResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(UnauthorizedResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(UnauthorizedResult))]
        public async Task PostWishlistItem_Test(PrivilegeLevel privilege, Type expected)
        {
            var dto = new WishlistItemDto { ClientId = 999, ProductId = 555, Quantity = 2 };
            var result = await _service.PostWishlistItem(dto, new UserData(1, privilege));
            Assert.IsType(expected, result.Result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(UnauthorizedResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(UnauthorizedResult))]
        public async Task PutWishlistItem_Test(PrivilegeLevel privilege, Type expected)
        {
            var item = _db.Set<WishlistItem>().First(w => w.ClientId == 1);
            var dto = new WishlistItemDto
            {
                ClientId = item.ClientId,
                ProductId = item.ProductId,
                Quantity = item.Quantity + 1
            };

            var result = await _service.PutWishlistItem(item.Id, dto, new UserData(1, privilege));
            Assert.IsType(expected, result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(UnauthorizedResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(UnauthorizedResult))]
        public async Task DeleteWishlistItem_Test(PrivilegeLevel privilege, Type expected)
        {
            var item = _db.Set<WishlistItem>().First(w => w.ClientId == 1);
            var result = await _service.DeleteWishlistItem(item.Id, new UserData(1, privilege));
            Assert.IsType(expected, result);
        }
    }
}
