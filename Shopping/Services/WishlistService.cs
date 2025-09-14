using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;

namespace Shopping.Services
{
    public class WishlistService
    {
        private readonly ShoppingDbContext _context;

        public WishlistService(ShoppingDbContext context)
        {
            _context = context;
        }

        //Get
        public async Task<IEnumerable<WishlistItemDto>> GetWishlistItems()
        {
            var wishlistItemsList = await _context.Set<WishlistItem>().ToListAsync();

            var wishlist = wishlistItemsList.Select(p => p.ToDto());

            return wishlist;
        }

        //Get by Id
        public async Task<ActionResult<WishlistItemDto>> GetWishlistItemById(int id)
        {
            var wishlistItem = await _context.Set<WishlistItem>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            if (wishlistItem == null)
            {
                return new NotFoundResult();
            }

            return wishlistItem.ToDto();
        }

        //Put
        public async Task<IActionResult> PutWishlistItem(int id, WishlistItemDto dto)
        {
            var entity = await _context.Set<WishlistItem>().FindAsync([id]);
            if (entity is null)
                return new NotFoundResult();

            entity.FromDto(id, dto);

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<WishlistItemDto>> PostWishlistItem(WishlistItemDto dto)
        {
            var entity = dto.ToEntity(0);

            _context.Set<WishlistItem>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(GetWishlistItemById), new { id = entity.Id }, dto);
        }

        //Delete
        public async Task<IActionResult> DeleteWishlistItem(int id)
        {
            var account = await _context.Wishlist.FindAsync(id);
            if (account == null)
            {
                return new NotFoundResult();
            }

            _context.Wishlist.Remove(account);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
