using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;
using Utility.Enums;

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
        public async Task<IEnumerable<WishlistItemDto>> GetWishlistItems(int? userId = null)
        {
            List<WishlistItem> wishlistItemsList;

            if (userId == null)
            {
                wishlistItemsList = await _context.Set<WishlistItem>().ToListAsync();
            }
            else
            {
                wishlistItemsList = await _context.Set<WishlistItem>()
                                                  .Where(w => w.ClientId == userId)
                                                  .ToListAsync();
            }

            var wishlist = wishlistItemsList.Select(p => p.ToDto());

            return wishlist;
        }

        //Get by Id
        public async Task<ActionResult<WishlistItemDto>> GetWishlistItemById(int id, int? clientId, PriviledgeLevel priviledgeLevel)
        {
            WishlistItem? wishlistItem;

            switch(priviledgeLevel)
            {
                case PriviledgeLevel.Admin:
                    wishlistItem = await _context.Set<WishlistItem>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();
                    break;
                case PriviledgeLevel.Customer:
                    if (clientId is null) return new BadRequestResult();
                    wishlistItem = await _context.Set<WishlistItem>().Where(c =>
                    c.Id.Equals(id) && c.ClientId.Equals(clientId)).SingleOrDefaultAsync();
                    break;
                default:
                    return new UnauthorizedResult();
            }

            if (wishlistItem == null)
            {
                return new NotFoundResult();
            }

            return wishlistItem.ToDto();
        }

        //Put
        public async Task<IActionResult> PutWishlistItem(int id, WishlistItemDto dto, int? clientId, PriviledgeLevel priviledgeLevel)
        {
            WishlistItem? entity;

            switch (priviledgeLevel)
            {
                case PriviledgeLevel.Admin:

                    entity = await _context.Set<WishlistItem>().FindAsync(id);

                    break;
                case PriviledgeLevel.Customer:


                    //Too tired...
                    //1. Czy jak wczesniej zadeklaruje entity, to bedzie ono dalej sledzone przez kontekst?
                    //2. Jak znalezc po PK i ClientId? Not sure if this is neccesary...


                    entity = await _context.Set<WishlistItem>().FindAsync(id, clientId);

                    break;
                default:
                    return new UnauthorizedResult();
            }


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
