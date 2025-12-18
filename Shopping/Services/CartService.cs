using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;
using Utility.Common;
using Utility.DtoEntity;
using Utility.Enums;

namespace Shopping.Services
{
    public class CartService
    {
        private readonly ShoppingDbContext _context;

        public CartService(ShoppingDbContext context)
        {
            _context = context;
        }

        //Get
        public async Task<ActionResult<IEnumerable<ShoppingCartItemDto>>> GetShoppingCartItems(UserData userData)
        {
            List<ShoppingCartItem> cartItemsList;

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                    cartItemsList = await _context.Set<ShoppingCartItem>().ToListAsync();
                    break;
                case PriviledgeLevel.Customer:
                    cartItemsList = await _context.Set<ShoppingCartItem>()
                        .Where(c => c.ClientId.Equals(userData.clientId)).ToListAsync();
                    break;
                default: return new ForbidResult();
            }

            var itemsList = cartItemsList.Select(p => p.ToDto());

            return new OkObjectResult(itemsList);
        }

        //Get by Id
        public async Task<ActionResult<ShoppingCartItemDto>> GetShoppingCartItemById(int id, UserData userData)
        {
            ShoppingCartItem? cartItem;

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                    cartItem = await _context.Set<ShoppingCartItem>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();
                    break;
                case PriviledgeLevel.Customer:
                    cartItem = await _context.Set<ShoppingCartItem>()
                        .Where(c => c.Id.Equals(id) && c.ClientId.Equals(userData.clientId)).SingleOrDefaultAsync();
                    break;
                default: return new ForbidResult();
            }

            if (cartItem == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(cartItem.ToDto());
        }


        //Get by ClientId
        public async Task<ActionResult<List<ShoppingCartItemDto>>> GetShoppingCartItemByClientId(int ClientId, UserData userData)
        {
            List<ShoppingCartItem>? order = await _context.Set<ShoppingCartItem>().Where(c => c.ClientId.Equals(ClientId)).ToListAsync();

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                //Nothing to do here
                case PriviledgeLevel.Customer:
                    if (ClientId != userData.clientId) return new UnauthorizedResult();
                    break;
                default: return new ForbidResult();
            }

            if (order == null)
            {
                return new NotFoundResult();
            }

            return order.Select(o => o.ToDto()).ToList();
        }

        //Put
        public async Task<IActionResult> PutShoppingCartItem(int id, ShoppingCartItemDto dto, UserData userData)
        {
            var entity = await _context.Set<ShoppingCartItem>().FindAsync(id);
            if (entity is null)
                return new NotFoundResult();

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                    entity.FromDto(id, dto);
                    break;
                case PriviledgeLevel.Customer:

                    if (entity.ClientId != userData.clientId) return new UnauthorizedResult();
                    entity.FromDto(id, dto);
                    break;
                default: return new ForbidResult();
            }

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<ShoppingCartItemDto>> PostShoppingCartItem(ShoppingCartItemDto dto, UserData userData)
        {
            ShoppingCartItem entity;

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                    entity = dto.ToEntity(0);
                    break;
                case PriviledgeLevel.Customer:
                    if (userData.clientId is null) return new ForbidResult();

                    entity = dto.ToEntity(0);
                    dto.ClientId = (int)userData.clientId;
                    break;
                default: return new ForbidResult();
            }

            _context.Set<ShoppingCartItem>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(GetShoppingCartItemById), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteShoppingCartItem(int id, UserData userData)
        {
            var cartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (cartItem == null)
            {
                return new NotFoundResult();
            }

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                    //Nothing to do here
                    break;
                case PriviledgeLevel.Customer:
                    if (cartItem.ClientId != userData.clientId) return new BadRequestResult();
                    break;
                default: return new ForbidResult();
            }

            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        //Delete by clientId | For this moment used only by Facade.Resolver 
        public async Task<IActionResult> DeleteShoppingCartItemsByClientId(int ClientId, UserData userData)
        {
            var cartItems = await _context.Set<ShoppingCartItem>().Where(c => c.ClientId.Equals(ClientId)).ToListAsync();
            if (cartItems == null)
            {
                return new NotFoundResult();
            }

            foreach (var item in cartItems)
            {
                _context.ShoppingCartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return new NoContentResult();
        }
    }
}

