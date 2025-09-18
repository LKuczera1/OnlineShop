using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;
using Utility.Common;
using Utility.DtoEntity;
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
        public async Task<ActionResult<WishlistItemDto>> GetWishlistItemById(int id, UserData userData)
        {
            WishlistItem? wishlistItem;

            switch(userData.priviledgeLevel)
            {
                case PriviledgeLevel.Admin:
                    wishlistItem = await _context.Set<WishlistItem>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();
                    break;
                case PriviledgeLevel.Customer:
                    if (userData.clientId is null) return new BadRequestResult();
                    wishlistItem = await _context.Set<WishlistItem>().Where(c =>
                    c.Id.Equals(id) && c.ClientId.Equals(userData.clientId)).SingleOrDefaultAsync();
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
        public async Task<IActionResult> PutWishlistItem(int id, WishlistItemDto dto, UserData userData)
        {
            var entity = await _context.Set<WishlistItem>().FindAsync(id);

            if (entity is null)
                return new NotFoundResult();

            switch (userData.priviledgeLevel)
            {
                case PriviledgeLevel.Admin:

                    entity.FromDto(id, dto);

                    break;
                case PriviledgeLevel.Customer:

                    if (userData.clientId is null)
                        return new UnauthorizedResult();

                    if (entity.ClientId != userData.clientId)
                        return new ForbidResult();

                    entity.FromDto(id, dto);
                    entity.ClientId = (int)userData.clientId;

                    break;
                default:
                    return new UnauthorizedResult();
            }

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<WishlistItemDto>> PostWishlistItem(WishlistItemDto dto, UserData userData)
        {
            var entity = dto.ToEntity(0);

            switch (userData.priviledgeLevel)
            {
                case PriviledgeLevel.Admin:

                    //Nothing to do here

                    break;
                case PriviledgeLevel.Customer:

                    if (userData.clientId is null)
                        return new UnauthorizedResult();

                    entity.ClientId = (int)userData.clientId;

                    break;
                default:
                    return new UnauthorizedResult();
            }

            _context.Set<WishlistItem>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(GetWishlistItemById), new { id = entity.Id }, dto);
        }

        //Delete
        public async Task<IActionResult> DeleteWishlistItem(int id, UserData userData)
        {
            var entity = await _context.Wishlist.FindAsync(id);
            if (entity == null)
            {
                return new NotFoundResult();
            }

            switch (userData.priviledgeLevel)
            {
                case PriviledgeLevel.Admin:

                    //Nothing to do here

                    break;
                case PriviledgeLevel.Customer:

                    if (userData.clientId is null)
                        return new UnauthorizedResult();

                    if (entity.ClientId != userData.clientId)
                        return new ForbidResult();

                    break;
                default:
                    return new UnauthorizedResult();
            }

            _context.Wishlist.Remove(entity);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
