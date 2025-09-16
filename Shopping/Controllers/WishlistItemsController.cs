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
using Utility.Enums;

namespace Shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistItemsController : ControllerBase
    {
        private readonly WishlistService _context;

        public WishlistItemsController(WishlistService context)
        {
            _context = context;
        }
        //
        //
        //
        // Trzeba dokonczyc resolver i endpointy tak aby uzytkownik dostawal tylko swoje rzeczy....
        // zabijcie mnie....
        // Jak rozdzielić endpointy między administratorów i customerów żeby nie robić pierdyliarda endpointów?
        //
        //
        //
        // Potrzebuje jednej metody w kontrolerach (Może nawet dziedziczonej z klasy)
        // ktora bedzie sprawdzac role uzytkownika i konwertowac ja na PriviledgeLevel
        //
        // Nastepnie Będzie to przekazywac do metody Services, gdzie za pomoca switch() uzytkownik bedzie
        // odpowiednio obslugiwany, lub w przypadku braku dostepu/dedykowanej obslugi zwracany "AccesDenied"
        //
        //
        private int? GetUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(value, out var id)) return id;

            return null;
        }

        private bool IsUserInRole(string role)
        {
            return User.IsInRole(role);
        }

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
            

            if(User.IsInRole(RolesStr.Admin))
            {
                return await _context.GetWishlistItemById(id,0, false);
            }
            else
            {
                return await _context.GetWishlistItemById(id, GetUserId(),false);
            }
        }

        // PUT: api/WishlistItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "GetWishlistItemById")]
        [Authorize(Roles = RolesStr.Admin)]
        public async Task<IActionResult> PutWishlistItem(int id, WishlistItemDto wishlistItem)
        {
            return await _context.PutWishlistItem(id, wishlistItem);
        }

        //// PUT: api/WishlistItems/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("PutWishlistItem{id}", Name = "GetWishlistItemById")]
        //[Authorize(Roles = RolesStr.Admin_Customer)]
        //public async Task<IActionResult> PutWishlistItemCustomer(int id, WishlistItemDto wishlistItem)
        //{
        //    if (GetUserId() != wishlistItem.ClientId) return BadRequest();
        //    return await _context.PutWishlistItem(id, wishlistItem);
        //}

        // POST: api/WishlistItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<ActionResult<WishlistItemDto>> PostWishlistItem(WishlistItemDto wishlistItem)
        {
            return await _context.PostWishlistItem(wishlistItem);
        }

        // DELETE: api/WishlistItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = RolesStr.Admin_Customer)]
        public async Task<IActionResult> DeleteWishlistItem(int id)
        {
            return await _context.DeleteWishlistItem(id);
        }

        /*
        private bool WishlistItemExists(int id)
        {
            return _context.Wishlist.Any(e => e.Id == id);
        }
        */
    }
}
