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
using System.Security.Claims;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Enums;

namespace Shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistItemsController : CustomControllerBase
    {
        private readonly WishlistService _context;

        public WishlistItemsController(WishlistService context)
        {
            _context = context;
        }
        //
        //
        //
        // Trzeba dokonczyc resolver i pozostałe kontrollery
        // Naprawić w services metode Getwishlistitem, i zrobic porządek w kontrolerze z operacją get
        //
        //

        // GET: api/WishlistItems
        [HttpGet]
        [Authorize(Roles = RolesStr.Customer)]
        public async Task<IEnumerable<WishlistItemDto>> GetWishlist()
        {
            //Fetches user wishlist  
            return await _context.GetWishlistItems(GetUserId());
        }

        // GET: api/WishlistItems/all
        [HttpGet("all")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IEnumerable<WishlistItemDto>> GetAllWishlist()
        {
            return await _context.GetWishlistItems();
        }

        // GET: api/WishlistItems/5
        [HttpGet("{id}")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult<WishlistItemDto>> GetWishlistItemById(int id)
        {
            return await _context.GetWishlistItemById(id, GetUserData());
        }

        // PUT: api/WishlistItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "GetWishlistItemById")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<IActionResult> PutWishlistItem(int id, WishlistItemDto wishlistItem)
        {
            return await _context.PutWishlistItem(id, wishlistItem, GetUserData());
        }

        // POST: api/WishlistItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult<WishlistItemDto>> PostWishlistItem(WishlistItemDto wishlistItem)
        {
            return await _context.PostWishlistItem(wishlistItem, GetUserData());
        }

        // DELETE: api/WishlistItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<IActionResult> DeleteWishlistItem(int id)
        {
            return await _context.DeleteWishlistItem(id, GetUserData());
        }

        /*
        private bool WishlistItemExists(int id)
        {
            return _context.Wishlist.Any(e => e.Id == id);
        }
        */
    }
}
