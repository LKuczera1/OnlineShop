using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping;
using Shopping.Dtos;
using Shopping.Models;
using Shopping.Services;

namespace Shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderedItemsController : ControllerBase
    {
        private readonly OrderService _context;

        public OrderedItemsController(OrderService context)
        {
            _context = context;
        }

        // GET: api/OrderedItems
        [HttpGet]
        public async Task<IEnumerable<OrderedItemDto>> GetOrderedItems()
        {
            return await _context.GetOrderItems();
        }

        // GET: api/OrderedItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderedItemDto>> GetOrderedItem(int id)
        {
            return await _context.GetOrderItemById(id);
        }

        // PUT: api/OrderedItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderedItem(int id, OrderedItemDto orderedItem)
        {
            return await _context.PutOrderItem(id, orderedItem);
        }

        // POST: api/OrderedItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderedItemDto>> PostOrderedItem(OrderedItemDto orderedItem)
        {
            return await _context.PostOrderItem(orderedItem);
        }

        // DELETE: api/OrderedItems/5
        [HttpDelete("{id}")]
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
