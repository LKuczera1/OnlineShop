using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;
using System.Collections;
using Utility.Common;
using Utility.DtoEntity;
using Utility.Enums;

namespace Shopping.Services
{
    public class OrderService
    {
        private readonly ShoppingDbContext _context;

        public OrderService(ShoppingDbContext context)
        {
            _context = context;
        }

        //Order Items CRUD-----------------------------------------------------------------------------------
        //Get
        public async Task<ActionResult<IEnumerable<OrderedItemDto>>> GetOrderItems(UserData userData, int? orderId = null)
        {
            List<OrderedItem>? orderItemsList;

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                    orderItemsList = await _context.Set<OrderedItem>().ToListAsync();
                    break;
                case PrivilegeLevel.SalesDepartmentWorker:

                    if (orderId is null)
                    {
                        return new BadRequestResult();
                    }
                    orderItemsList = await _context.Set<OrderedItem>().Where(i => i.OrderId.Equals(orderId)).ToListAsync();

                    break;
                case PrivilegeLevel.Customer:

                    if (orderId is null)
                    {
                        return new BadRequestResult();
                    }

                    var order = await GetOrderById((int)orderId, userData);

                    if(order.Result is ForbidResult or NotFoundResult || order.Value is null) return new BadRequestResult();

                    orderItemsList = await _context.Set<OrderedItem>()
                        .Where(i => i.OrderId.Equals(order.Value.OrderId)).ToListAsync();

                    break;
                default: return new ForbidResult();
            }


            var orderItems = orderItemsList.Select(p => p.ToDto());

            return new OkObjectResult(orderItems);
        }

        //Get by Id
        public async Task<ActionResult<OrderedItemDto>> GetOrderItemById(int id, UserData userData, int? orderId = null)
        {
            OrderedItem? orderItem;

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                case PrivilegeLevel.SalesDepartmentWorker:
                    orderItem = await _context.Set<OrderedItem>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

                    break;
                case PrivilegeLevel.Customer:

                    if (orderId is null)
                    {
                        return new BadRequestResult();
                    }

                    var order = await GetOrderById((int)orderId, userData);

                    if (order.Result is ForbidResult or NotFoundResult || order.Value is null) return new BadRequestResult();

                    orderItem = await _context.Set<OrderedItem>().Where(c => c.Id.Equals(id) 
                        && c.OrderId.Equals(order.Value.ClientId)).SingleOrDefaultAsync();

                    break;
                default: return new ForbidResult();
            }

            if (orderItem == null)
            {
                return new NotFoundResult();
            }

            return orderItem.ToDto();
        }

        //Put
        public async Task<IActionResult> PutOrderItem(int id, OrderedItemDto dto, UserData userData)
        {
            var entity = await _context.Set<OrderedItem>().FindAsync(id);
            if (entity is null)
                return new NotFoundResult();

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:

                    entity.FromDto(id, dto);
                    break;
                case PrivilegeLevel.SalesDepartmentWorker:

                    var temp = entity.OrderId;
                    entity.FromDto(id, dto);
                    entity.OrderId = temp;

                    break;
                default: return new ForbidResult();
            }

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<OrderedItemDto>> PostOrderItem(OrderedItemDto dto, UserData? userData = null)
        {
            var entity = dto.ToEntity(0);


            _context.Set<OrderedItem>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(GetOrderItemById), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteOrderItem(int id, UserData? userData = null)
        {
            var orderItem = await _context.OrderedItems.FindAsync(id);
            if (orderItem == null)
            {
                return new NotFoundResult();
            }

            _context.OrderedItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        //Order CRUD-----------------------------------------------------------------------------------
        //Get
        public async Task<IEnumerable<OrderDto>> GetOrders(UserData userData)
        {
            List<Order> ordersList;

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                    ordersList = await _context.Set<Order>().ToListAsync();
                    break;
                case PrivilegeLevel.SalesDepartmentWorker:
                    ordersList = await _context.Set<Order>().ToListAsync();
                    break;
                default: 
                    ordersList = new List<Order>();
                    ordersList.Clear();
                    break;
            }

            var orders = ordersList.Select(p => p.ToDto());

            return orders;
        }

        //Get by Id
        public async Task<ActionResult<OrderDto>> GetOrderById(int id, UserData userData)
        {
            Order? order;

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                case PrivilegeLevel.SalesDepartmentWorker:
                    order = await _context.Set<Order>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();
                    break;
                default: return new ForbidResult();
            }

            if (order == null)
            {
                return new NotFoundResult();
            }

            return order.ToDto();
        }

        //Put
        public async Task<IActionResult> PutOrder(int id, OrderDto dto, UserData userData)
        {
            Order? entity;

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                case PrivilegeLevel.SalesDepartmentWorker:
                    entity = await _context.Set<Order>().FindAsync([id]);
                    break;
                default: return new ForbidResult();
            }

            if (entity is null)
                return new NotFoundResult();

            entity.FromDto(id, dto);

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<OrderDto>> PostOrder(OrderDto dto, UserData userData)
        {
            var entity = dto.ToEntity(0);

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                case PrivilegeLevel.SalesDepartmentWorker:
                    _context.Set<Order>().Add(entity);
                    await _context.SaveChangesAsync();
                    break;
                default: return new ForbidResult();
            }


            return new CreatedAtRouteResult(nameof(GetOrderById), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteOrder(int id, UserData userData)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new NotFoundResult();
            }

            switch (userData.priviledgeLevel)
            {
                case PrivilegeLevel.Admin:
                case PrivilegeLevel.SalesDepartmentWorker:

                    break;
                case PrivilegeLevel.Customer:
                    if (order.ClientId != userData.clientId) return new ForbidResult();
                    break;
                default: return new ForbidResult();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

    }
}
