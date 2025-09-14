using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Models;

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
        public async Task<IEnumerable<ShoppingCartItemDto>> GetShoppingCartItems()
        {
            var cartItemsList = await _context.Set<ShoppingCartItem>().ToListAsync();

            var itemsList = cartItemsList.Select(p => p.ToDto());

            return itemsList;
        }

        //Get by Id
        public async Task<ActionResult<ShoppingCartItemDto>> GetShoppingCartItemById(int id)
        {
            var cartItem = await _context.Set<ShoppingCartItem>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

            if (cartItem == null)
            {
                return new NotFoundResult();
            }

            return cartItem.ToDto();
        }


        //Get by ClientId
        public async Task<ActionResult<List<ShoppingCartItemDto>>> GetShoppingCartItemByClientId(int ClientId)
        {
            var order = await _context.Set<ShoppingCartItem>().Where(c => c.ClientId.Equals(ClientId)).ToListAsync();

            if (order == null)
            {
                return new NotFoundResult();
            }

            return order.Select(o => o.ToDto()).ToList();
        }

        //Put
        public async Task<IActionResult> PutShoppingCartItem(int id, ShoppingCartItemDto dto)
        {
            var entity = await _context.Set<ShoppingCartItem>().FindAsync([id]);
            if (entity is null)
                return new NotFoundResult();

            entity.FromDto(id, dto);

            //_context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        //Post
        public async Task<ActionResult<ShoppingCartItemDto>> PostShoppingCartItem(ShoppingCartItemDto dto)
        {
            var entity = dto.ToEntity(0);

            _context.Set<ShoppingCartItem>().Add(entity);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult(nameof(GetShoppingCartItemById), new { id = entity.Id }, entity);
        }

        //Delete
        public async Task<IActionResult> DeleteShoppingCartItem(int id)
        {
            var cartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (cartItem == null)
            {
                return new NotFoundResult();
            }

            _context.ShoppingCartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        //Delete by clientId
        public async Task<IActionResult> DeleteShoppingCartItemByClientId(int ClientId)
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
