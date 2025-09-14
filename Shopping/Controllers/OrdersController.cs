using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping;
using Shopping.Dtos;
using Shopping.Enums;
using Shopping.Models;
using Shopping.Services;
using Shopping.Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _context;
        private readonly ShoppingFacade _shoppingFacade;

        public OrdersController(OrderService context, ShoppingFacade shoppingFacade)
        {
            _context = context;
            _shoppingFacade = shoppingFacade;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<IEnumerable<OrderDto>> GetOrders()
        {
            return await _context.GetOrders();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            return await _context.GetOrderById(id);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderDto order)
        {
            return await _context.PutOrder(id, order);
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDto>> PostOrder(OrderDto order)
        {
            return await _context.PostOrder(order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            return await _context.DeleteOrder(id);
        }

        //Facade

        [HttpPost("{itemId}:move-to-cart")]
        public async Task<ActionResult> MoveItemFromWishlistToCart(int itemId)
        {
            return await _shoppingFacade.MoveItemFromWishlistToCart(itemId);
        }

        [HttpPost("place-order")]
        public async Task<ActionResult> PlaceOrder()
        {
            //-KLIENT ID TRZEBA PODAC
            return await _shoppingFacade.PlaceOrder(10);
        }

        [HttpGet("{orderId}/status")]
        public async Task<ActionResult<OrderStatusDto>> GetOrderStatus(int orderId)
        {
            return await _shoppingFacade.GetOrderStatus(orderId);
        }

        /*
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
        */
    }
}
