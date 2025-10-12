using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping;
using Shopping.Dtos;
using Shopping.Models;
using Shopping.Services;
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
    public class OrderedItemsController : CustomControllerBase
    {
        private readonly OrderService _context;

        public OrderedItemsController(OrderService context)
        {
            _context = context;
        }

        // GET: api/OrderedItems
        [HttpGet("{orderId?}")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker_Customer)]
        public async Task<ActionResult<IEnumerable<OrderedItemDto>>> GetOrderedItems(int? orderId)
        {
            return await _context.GetOrderItems(GetUserData(), orderId);
        }

        // GET: api/OrderedItems/5
        [HttpGet("{id}/{orderId?}")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker_Customer)]
        public async Task<ActionResult<OrderedItemDto>> GetOrderedItem(int id, int? orderId)
        {
            return await _context.GetOrderItemById(id, GetUserData(), orderId);
        }

        // PUT: api/OrderedItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "GetOrderedItem")]
        [Authorize(Roles = RolesStr.Admin_SalesDepartmentWorker)]
        public async Task<IActionResult> PutOrderedItem(int id, OrderedItemDto orderedItem)
        {
            return await _context.PutOrderItem(id, orderedItem, GetUserData());
        }

        // POST: api/OrderedItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<ActionResult<OrderedItemDto>> PostOrderedItem(OrderedItemDto orderedItem)
        {
            return await _context.PostOrderItem(orderedItem);
        }

        // DELETE: api/OrderedItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IActionResult> DeleteOrderedItem(int id)
        {
            return await _context.DeleteOrderItem(id);
        }

        /*
        private bool OrderedItemExists(int id)
        {
            return _context.OrderedItems.Any(e => e.Id == id);
        }
        */
    }
}

