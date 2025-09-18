using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Shopping.Dtos;
using Shopping.Enums;
using Shopping.Models;
using Shopping.Resolvers;

namespace Shopping.Services.Facade
{
    //Połączenie pomiędzy serwisami umożliwiające np przenoszenie przedmiotów z wishlist do koszyka
    public class ShoppingFacade
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly WishlistService _wishlistService;

        private readonly CatalogResolver _catalogResolver;

        public ShoppingFacade(CartService cartService, OrderService orderService, WishlistService wishlistService, CatalogResolver catalogResolver)
        {
            _cartService = cartService;
            _orderService = orderService;
            _wishlistService = wishlistService;

            _catalogResolver = catalogResolver;

            _catalogResolver.ResolveForProduct(1);
        }

        public async Task<ActionResult> MoveItemFromWishlistToCart(int itemId)
        {

            //Nie by product id tylko by customerid
            var wishlistItem = await _wishlistService.GetWishlistItemById(itemId, null);

            if (wishlistItem is NotFoundResult)
            {
                return new NotFoundResult();
            }

            var product = _catalogResolver.ResolveForProduct(wishlistItem.Value!.ProductId).Result;

            if(product == null)
            {
                return new NotFoundResult();
            }

            var item = new ShoppingCartItemDto()
            {
                ClientId = wishlistItem.Value!.ClientId,
                ProductId = wishlistItem.Value!.ProductId,
                Quantity = wishlistItem.Value!.Quantity,
                Price = product.Price,
            };

            await _wishlistService.DeleteWishlistItem(itemId, null);
            await _cartService.PostShoppingCartItem(item);

            return new OkResult();
        }

        public async Task<ActionResult> PlaceOrder(int cliendId)
        {
            var clientOrder = _cartService.GetShoppingCartItemByClientId(cliendId);

            if (clientOrder.Result is NotFoundResult)
            {
                return new BadRequestResult();
            }

            double totalValue = 0;

            foreach (var item in clientOrder.Result.Value)
            {
                totalValue += item.Price;
            }

            var order = new OrderDto();
            order.ClientId = cliendId;
            order.OrderTime = DateTime.Now;
            order.Status = OrderStatus.Paid;
            order.TotalPrice = totalValue;

            var result = await _orderService.PostOrder(order);

            int orderId = result.Value.OrderId;

            foreach (var item in clientOrder.Result.Value)
            {
                await _orderService.PostOrderItem(item.ToOrderedItemDto(orderId));
            }

            await _cartService.DeleteShoppingCartItemByClientId(cliendId);

            return new OkResult();
        }

        public async Task<ActionResult<OrderStatusDto>> GetOrderStatus(int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);

            if (order is NotFoundResult)
            {
                return new NotFoundResult();
            }

            return order.Value.ToOrderStatus();
        }
    }
}
