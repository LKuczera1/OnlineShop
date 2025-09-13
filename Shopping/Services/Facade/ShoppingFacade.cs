using Microsoft.AspNetCore.Mvc;
using Shopping.Enums;

namespace Shopping.Services.Facade
{
    //Połączenie pomiędzy serwisami umożliwiające np przenoszenie przedmiotów z wishlist do koszyka
    public class ShoppingFacade
    {
        private readonly CartService _cartService;
        private readonly OrderService _orderService;
        private readonly WishlistService _wishlistService;

        public ShoppingFacade(CartService cartService, OrderService orderService, WishlistService wishlistService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _wishlistService = wishlistService;
        }

        public async Task<ActionResult> MoveItemFromWishlistToCart(int itemId)
        {
            return new OkResult();
        }

        public async Task<ActionResult> PlaceOrder()
        {
            return new OkResult();
        }

        public async Task<ActionResult<OrderStatus>> GetOrderStatus(int orderId)
        {
            return new OkResult();
        }
    }
}
