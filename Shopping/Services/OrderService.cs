using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;

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
        public async Task<IEnumerable<OrderedItemDto>> GetOrderItems()
        {
            var orderItemsList = await _context.Set<OrderedItem>().ToListAsync();

            var orderItems = orderItemsList.Select(p => p.ToDto());

            return orderItems;
        }

        //Get by Id
        public async Task<ActionResult<OrderedItemDto>> GetOrderItemById(int id)
        {
            var orderItem = await _context.Set<OrderedItem>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            if (orderItem == null)
            {
                return new NotFoundResult();
            }

            return orderItem.ToDto();
        }

        //Put
        public async Task<IActionResult> PutOrderItem(int id, OrderedItemDto dto)
        {
            var entity = await _context.Set<OrderedItem>().FindAsync([id]);
            if (entity is null)
                return new NotFoundResult();

            entity.FromDto(id, dto);

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<OrderedItemDto>> PostOrderItem(OrderedItemDto dto)
        {
            var entity = dto.ToEntity(0);

            _context.Set<OrderedItem>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(GetOrderItemById), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteOrderItem(int id)
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
        public async Task<IEnumerable<OrderDto>> GetOrders()
        {
            var ordersList = await _context.Set<Order>().ToListAsync();

            var orders = ordersList.Select(p => p.ToDto());

            return orders;
        }

        //Get by Id
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _context.Set<Order>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            if (order == null)
            {
                return new NotFoundResult();
            }

            return order.ToDto();
        }

        //Put
        public async Task<IActionResult> PutOrder(int id, OrderDto dto)
        {
            var entity = await _context.Set<Order>().FindAsync([id]);
            if (entity is null)
                return new NotFoundResult();

            entity.FromDto(id, dto);

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<OrderDto>> PostOrder(OrderDto dto)
        {
            var entity = dto.ToEntity(0);

            _context.Set<Order>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(GetOrderById), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return new NotFoundResult();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

    }
}
