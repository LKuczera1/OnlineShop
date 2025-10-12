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
    public class OrderServiceTests : ServicesTestsBase
    {
        private readonly ShoppingDbContext _db;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            var options = new DbContextOptionsBuilder<ShoppingDbContext>()
                .UseInMemoryDatabase($"OrdersTestDB_{Guid.NewGuid()}")
                .Options;

            _db = new ShoppingDbContext(options);
            _service = new OrderService(_db);

            // Seed Orders
            var ordersJson = loadDbSource("OrderDb.json");
            var orders = JsonSerializer.Deserialize<List<Order>>(ordersJson!) ?? new();
            _db.Set<Order>().AddRange(orders);

            // Seed OrderedItems
            var itemsJson = loadDbSource("OrderedItemDb.json");
            var items = JsonSerializer.Deserialize<List<OrderedItem>>(itemsJson!) ?? new();
            _db.Set<OrderedItem>().AddRange(items);

            _db.SaveChanges();
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin)]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker)]
        public async Task GetOrders_AdminOrSales_ShouldReturnAll(PrivilegeLevel privilege)
        {
            var list = await _service.GetOrders(new UserData(0, privilege));
            Assert.NotNull(list);
            Assert.Equal(10, list.Count());
        }

        [Theory]
        [InlineData(PrivilegeLevel.Customer, 1)]
        [InlineData(PrivilegeLevel.Customer, 2)]
        [InlineData(PrivilegeLevel.Customer, 3)]
        public async Task GetOrders_Customer_ShouldReturnOnlyOwn(PrivilegeLevel privilege, int clientId)
        {
            var list = await _service.GetOrders(new UserData(clientId, privilege));
            Assert.NotNull(list);
            Assert.All(list, o => Assert.Equal(clientId, o.ClientId));
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(OkObjectResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(OkObjectResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(NotFoundResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task GetOrderById_Test(PrivilegeLevel privilege, Type expected)
        {
            var any = _db.Set<Order>().AsQueryable().Select(o => o.Id).First();

            var result = await _service.GetOrderById(any, new UserData(1, privilege));

            if (result.Result is null)
                Assert.True(result.Value != null);
            else
                Assert.IsType(expected, result.Result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(CreatedAtRouteResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PostOrder_Test(PrivilegeLevel privilege, Type expected)
        {
            var nextOrderId = _db.Set<Order>().Max(o => o.OrderId) + 1;
            var dto = new OrderDto
            {
                OrderId = nextOrderId,
                ClientId = 1,
                Status = Shopping.Enums.OrderStatus.Paid,
                TotalPrice = 123.45,
                OrderTime = DateTime.UtcNow,
            };

            var result = await _service.PostOrder(dto, new UserData(0, privilege));
            Assert.IsType(expected, result.Result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PutOrder_Test(PrivilegeLevel privilege, Type expected)
        {
            var entity = _db.Set<Order>().First();
            var dto = new OrderDto
            {
                OrderId = entity.OrderId,
                ClientId = entity.ClientId,
                Status = Shopping.Enums.OrderStatus.InDelivery,
                TotalPrice = entity.TotalPrice + 10,
                OrderTime = entity.OrderTime,
                PackedTime = DateTime.UtcNow
            };

            var result = await _service.PutOrder(entity.Id, dto, new UserData(entity.ClientId, privilege));
            Assert.IsType(expected, result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task DeleteOrder_Test(PrivilegeLevel privilege, Type expected)
        {
            var id = _db.Set<Order>().Select(o => o.Id).First();
            var result = await _service.DeleteOrder(id, new UserData(0, privilege));
            Assert.IsType(expected, result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PostOrderItem_Test(PrivilegeLevel privilege, Type expected)
        {
            var order = _db.Set<Order>().First();
            var dto = new OrderedItemDto
            {
                OrderId = order.OrderId,
                ProductId = 999,
                Quantity = 2,
                Price = 19.99
            };

            var result = await _service.PostOrderItem(dto, new UserData(0, privilege));
            Assert.IsType(expected, result.Result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PutOrderItem_Test(PrivilegeLevel privilege, Type expected)
        {
            var item = _db.Set<OrderedItem>().First();
            var dto = new OrderedItemDto
            {
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity + 1,
                Price = item.Price + 5
            };

            var result = await _service.PutOrderItem(item.Id, dto, new UserData(0, privilege));
            Assert.IsType(expected, result);
        }

        [Theory]
        [InlineData(PrivilegeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PrivilegeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PrivilegeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task DeleteOrderItem_Test(PrivilegeLevel privilege, Type expected)
        {
            var itemId = _db.Set<OrderedItem>().Select(i => i.Id).First();

            var result = await _service.DeleteOrderItem(itemId, new UserData(0, privilege));

            Assert.IsType(expected, result);
        }
    }
}
