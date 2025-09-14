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
    public class ShoppingCartItemsController : ControllerBase
    {
        private readonly CartService _context;

        public ShoppingCartItemsController(CartService context)
        {
            _context = context;
        }

        // GET: api/ShoppingCartItems
        [HttpGet]
        public async Task<IEnumerable<ShoppingCartItemDto>> GetShoppingCartItems()
        {
            return await _context.GetShoppingCartItems();
        }

        // GET: api/ShoppingCartItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingCartItemDto>> GetShoppingCartItemById(int id)
        {
            return await _context.GetShoppingCartItemById(id);
        }

        // PUT: api/ShoppingCartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "GetShoppingCartItemById")]
        public async Task<IActionResult> PutShoppingCartItem(int id, ShoppingCartItemDto shoppingCartItem)
        {
            return await _context.PutShoppingCartItem(id, shoppingCartItem);
        }

        // POST: api/ShoppingCartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShoppingCartItemDto>> PostShoppingCartItem(ShoppingCartItemDto shoppingCartItem)
        {
            return await _context.PostShoppingCartItem(shoppingCartItem);
        }

        // DELETE: api/ShoppingCartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCartItem(int id)
        {
            return await _context.DeleteShoppingCartItem(id);
        }

        /*
        private bool ShoppingCartItemExists(int id)
        {
            return _context.ShoppingCartItem.Any(e => e.Id == id);
        }
        */
    }
}
