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
    public class ShoppingCartItemsController : CustomControllerBase
    {
        private readonly CartService _context;

        public ShoppingCartItemsController(CartService context)
        {
            _context = context;
        }

        // GET: api/ShoppingCartItems
        [HttpGet]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult<IEnumerable<ShoppingCartItemDto>>> GetShoppingCartItems()
        {
            return await _context.GetShoppingCartItems(GetUserData());
        }

        // GET: api/ShoppingCartItems/5
        [HttpGet("{id}")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult<ShoppingCartItemDto>> GetShoppingCartItemById(int id)
        {
            return await _context.GetShoppingCartItemById(id, GetUserData());
        }

        // GET: api/ShoppingCartItems/5
        [HttpPost("PlaceOrder")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<IActionResult> SetOrder([FromBody] List<SetOrderDto> items)
        {
            return await _context.PlaceOrder(items, GetUserData());
        }

        // PUT: api/ShoppingCartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "GetShoppingCartItemById")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<IActionResult> PutShoppingCartItem(int id, ShoppingCartItemDto shoppingCartItem)
        {
            return await _context.PutShoppingCartItem(id, shoppingCartItem, GetUserData());
        }

        // POST: api/ShoppingCartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult<ShoppingCartItemDto>> PostShoppingCartItem(ShoppingCartItemDto shoppingCartItem)
        {
            return await _context.PostShoppingCartItem(shoppingCartItem, GetUserData());
        }

        [HttpPost("{productId}/{quantity}")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult<ShoppingCartItemDto>> PostShoppingCartItem(int productId, int quantity)
        {
            return await _context.PostShoppingCartItem(productId, quantity, GetUserData());
        }

        // DELETE: api/ShoppingCartItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<IActionResult> DeleteShoppingCartItem(int id)
        {
            return await _context.DeleteShoppingCartItem(id, GetUserData());
        }
    }
}

