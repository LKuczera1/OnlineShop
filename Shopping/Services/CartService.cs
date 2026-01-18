using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Dtos;
using Shopping.Enums;
using Shopping.Models;
using Shopping.Resolvers;
using Shopping.Services.Facade;
using Utility.Common;
using Utility.DtoEntity;
using Utility.Enums;

namespace Shopping.Services
{
    public class CartService
    {
        private readonly ShoppingDbContext _context;
        private readonly CatalogResolver _catalogResolver;
        private readonly OrderService _orderService;

        public CartService(ShoppingDbContext context, CatalogResolver catalogResolver, OrderService orderService)
        {
            _context = context;
            _catalogResolver = catalogResolver;
            _orderService = orderService;
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


        public async Task<ActionResult<ShoppingCartItemDto>> PostShoppingCartItem(int prodId, double quantity, UserData userData)
        {
            ShoppingCartItem entity;

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                case PriviledgeLevel.Customer:
                    if (userData.clientId is null) return new ForbidResult();

                    var resolvedProduct = await _catalogResolver.ResolveForProduct(prodId);

                    if (resolvedProduct == null) return new BadRequestResult();

                    entity = new ShoppingCartItem();

                    entity.Price = resolvedProduct.Price;
                    entity.ProductId = prodId;
                    entity.Quantity = quantity;
                    entity.ClientId = (int)userData.clientId;
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

            return new OkResult();
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

        public async Task<IActionResult> PlaceOrder(List<SetOrderDto> items, UserData userData)
        {
            if(items.Count == 0) return new BadRequestResult();

            switch (userData.privilegeLevel)
            {
                case PriviledgeLevel.Admin:
                case PriviledgeLevel.Customer:


                    var cartItems = await _context.Set<ShoppingCartItem>().Where(c => c.ClientId.Equals(userData.clientId)).ToListAsync();

                    if(cartItems == null) return new BadRequestResult();
                    if(cartItems.Count != items.Count) return new BadRequestResult();

                    while(items.Count > 0) 
                    {
                        var searchResult = cartItems.Find(c => c.Id.Equals(items[0].Id));
                        if (searchResult == null) return new BadRequestResult();

                        searchResult.Quantity = items[0].Quantity;

                        items.RemoveAt(0);
                    }

                    await _context.SaveChangesAsync();

                    return await PlaceOrder(userData);

                    break;
                default: return new ForbidResult();
            }
        }

        private async Task<ActionResult> PlaceOrder(UserData userData)
        {
            if (userData.clientId is null) return new BadRequestResult();

            var clientOrder = await GetShoppingCartItemByClientId((int)userData.clientId, userData);

            if (clientOrder.Result is NotFoundObjectResult || clientOrder.Value is null)
            {
                return new BadRequestResult();
            }

            double totalValue = 0;

            foreach (var item in clientOrder.Value)
            {
                totalValue += item.Price;
            }

            var order = new OrderDto();
            order.ClientId = (int)userData.clientId;
            order.OrderTime = DateTime.Now;
            order.Status = OrderStatus.Paid;
            order.TotalPrice = totalValue;


            //Temporary workaround for permissions issue
            var result = await _orderService.PostOrder(order, new UserData(userData.clientId, PriviledgeLevel.Admin));

            int orderId = 0;



            if (result.Result is CreatedAtRouteResult created && created.Value is Shopping.Models.Order entity)
            {
                orderId = entity.Id;
            }
            else
            {
                return new BadRequestObjectResult("Nie udało się pobrać ID zamówienia.");
            }

            foreach (var item in clientOrder.Value)
            {
                await _orderService.PostOrderItem(item.ToOrderedItemDto(orderId), userData);
            }

            var finalResult = await DeleteShoppingCartItemsByClientId((int)userData.clientId, userData);

            if (finalResult is NotFoundResult) return new OkObjectResult("An unexpected error probably happend.");

            return new OkResult();
        }
    }
}

