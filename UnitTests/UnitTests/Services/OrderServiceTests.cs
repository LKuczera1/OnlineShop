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
        [InlineData(PriviledgeLevel.Admin)]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker)]
        public async Task GetOrders_AdminOrSales_ShouldReturnAll(PriviledgeLevel privilege)
        {
            var list = await _service.GetOrders(new UserData(0, privilege));
            Assert.NotNull(list);
            Assert.Equal(10, list.Count());
        }

        [Theory]
        [InlineData(PriviledgeLevel.Customer, 1)]
        [InlineData(PriviledgeLevel.Customer, 2)]
        [InlineData(PriviledgeLevel.Customer, 3)]
        public async Task GetOrders_Customer_ShouldReturnOnlyOwn(PriviledgeLevel privilege, int clientId)
        {
            var list = await _service.GetOrders(new UserData(clientId, privilege));
            Assert.NotNull(list);
            Assert.All(list, o => Assert.Equal(clientId, o.ClientId));
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, typeof(OkObjectResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(OkObjectResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(NotFoundResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task GetOrderById_Test(PriviledgeLevel privilege, Type expected)
        {
            var any = _db.Set<Order>().AsQueryable().Select(o => o.Id).First();

            var result = await _service.GetOrderById(any, new UserData(1, privilege));

            if (result.Result is null)
                Assert.True(result.Value != null);
            else
                Assert.IsType(expected, result.Result);
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(CreatedAtRouteResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PostOrder_Test(PriviledgeLevel privilege, Type expected)
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
        [InlineData(PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PutOrder_Test(PriviledgeLevel privilege, Type expected)
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
        [InlineData(PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task DeleteOrder_Test(PriviledgeLevel privilege, Type expected)
        {
            var id = _db.Set<Order>().Select(o => o.Id).First();
            var result = await _service.DeleteOrder(id, new UserData(0, privilege));
            Assert.IsType(expected, result);
        }

        [Theory]
        [InlineData(PriviledgeLevel.Admin, typeof(CreatedAtRouteResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PostOrderItem_Test(PriviledgeLevel privilege, Type expected)
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
        [InlineData(PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task PutOrderItem_Test(PriviledgeLevel privilege, Type expected)
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
        [InlineData(PriviledgeLevel.Admin, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.SalesDepartmentWorker, typeof(NoContentResult))]
        [InlineData(PriviledgeLevel.Customer, typeof(ForbidResult))]
        [InlineData(PriviledgeLevel.NotAssigned, typeof(ForbidResult))]
        public async Task DeleteOrderItem_Test(PriviledgeLevel privilege, Type expected)
        {
            var itemId = _db.Set<OrderedItem>().Select(i => i.Id).First();

            var result = await _service.DeleteOrderItem(itemId, new UserData(0, privilege));

            Assert.IsType(expected, result);
        }
    }
}
