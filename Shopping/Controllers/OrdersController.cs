using Microsoft.AspNetCore.Authorization;
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
using Utility.Common;
using Utility.Enums;

namespace Shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : CustomControllerBase
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
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker_Customer)]
        public async Task<IEnumerable<OrderDto>> GetOrders()
        {
            return await _context.GetOrders(GetUserData());
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker_Customer)]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            return await _context.GetOrderById(id, GetUserData());
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "GetOrderById")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        public async Task<IActionResult> PutOrder(int id, OrderDto order)
        {
            return await _context.PutOrder(id, order, GetUserData());
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        public async Task<ActionResult<OrderDto>> PostOrder(OrderDto order)
        {
            return await _context.PostOrder(order, GetUserData());
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            return await _context.DeleteOrder(id, GetUserData());
        }

        //Facade

        [HttpPost("{itemId}:move-to-cart")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult> MoveItemFromWishlistToCart(int itemId)
        {
            return await _shoppingFacade.MoveItemFromWishlistToCart(itemId, GetUserData());
        }

        [HttpPost("place-order")]
        [Authorize(Roles = RolesStr.Customer)]
        public async Task<ActionResult> PlaceOrder()
        {
            return await _shoppingFacade.PlaceOrder(GetUserData());
        }

        [HttpPut("{orderId}/set-status/{status}")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        public async Task<ActionResult> SetOrderStatus(int orderId, OrderStatus status)
        {
            return await _shoppingFacade.SetOrderStatus(orderId, GetUserData(), status);
        }

        [HttpGet("{orderId}/status")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker_Customer)]
        public async Task<ActionResult<OrderStatusDto>> GetOrderStatus(int orderId)
        {
            return await _shoppingFacade.GetOrderStatus(orderId, GetUserData());
        }

        /*
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
        */
    }
}
