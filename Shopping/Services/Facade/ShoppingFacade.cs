using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using Shopping.Dtos;
using Shopping.Enums;
using Shopping.Models;
using Shopping.Resolvers;
using Utility.Common;
using Utility.Enums;

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
        }

        public async Task<ActionResult> MoveItemFromWishlistToCart(int itemId, UserData userData)
        {
            var wishlistItem = await _wishlistService.GetWishlistItemById(itemId, userData);

            if (wishlistItem.Result is NotFoundResult || wishlistItem is null || wishlistItem.Result is BadRequestResult)
            {
                return new NotFoundResult();
            }

            var product = _catalogResolver.ResolveForProduct(wishlistItem.Value!.ProductId).Result;

            if (product == null)
            {
                return new NotFoundResult();
            }

            ShoppingCartItemDto item;

            switch (userData.privilegeLevel)
            {
                case PrivilegeLevel.Admin:


                    item = new ShoppingCartItemDto()
                    {
                        ClientId = wishlistItem.Value!.ClientId,
                        ProductId = wishlistItem.Value!.ProductId,
                        Quantity = wishlistItem.Value!.Quantity,
                        Price = product.Price,
                    };

                    var mockUser = new UserData(item.ClientId, PrivilegeLevel.Customer);

                    await _wishlistService.DeleteWishlistItem(itemId, mockUser);

                    break;
                case PrivilegeLevel.Customer:

                    item = new ShoppingCartItemDto()
                    {
                        ClientId = wishlistItem.Value!.ClientId,
                        ProductId = wishlistItem.Value!.ProductId,
                        Quantity = wishlistItem.Value!.Quantity,
                        Price = product.Price,
                    };

                    await _wishlistService.DeleteWishlistItem(itemId, userData);

                    break;
                default: return new ForbidResult();
            }

            await _cartService.PostShoppingCartItem(item, userData);

            return new OkResult();
        }

        public async Task<ActionResult> PlaceOrder(UserData userData)
        {
            if (userData.clientId is null || userData.privilegeLevel != PrivilegeLevel.Customer) return new BadRequestResult();

            var clientOrder = await _cartService.GetShoppingCartItemByClientId((int)userData.clientId, userData);

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

            var result = await _orderService.PostOrder(order, userData);

            int orderId = result.Value.OrderId;

            foreach (var item in clientOrder.Value)
            {
                await _orderService.PostOrderItem(item.ToOrderedItemDto(orderId));
            }

            var finalResult = await _cartService.DeleteShoppingCartItemsByClientId((int)userData.clientId, userData);

            if (finalResult is NotFoundResult) return new OkObjectResult("An unexpected error probably happend.");

            return new OkResult();
        }

        public async Task<ActionResult<OrderStatusDto>> GetOrderStatus(int orderId, UserData userData)
        {
            var order = await _orderService.GetOrderById(orderId, userData);

            if (order.Result is NotFoundObjectResult || order is null)
            {
                return new NotFoundResult();
            }

            if (order.Value.ClientId != (int)userData.clientId
                && userData.privilegeLevel == PrivilegeLevel.Customer) return new BadRequestResult();

            return order.Value.ToOrderStatus();
        }
        public async Task<ActionResult> SetOrderStatus(int orderId, UserData userData, OrderStatus status)
        {
            var order = await _orderService.GetOrderById(orderId, userData);

            if (order.Result is NotFoundObjectResult || order is null)
            {
                return new NotFoundResult();
            }

            switch (userData.privilegeLevel)
            {
                case PrivilegeLevel.Admin:
                case PrivilegeLevel.SalesDepartmentWorker:

                    if (!IsOrderStatusInRange(status)) return new BadRequestObjectResult("Invalid order status.");

                    if (order.Value.Status > status || order.Value.Status + 1 != status) return new BadRequestObjectResult("Invalid order status.");

                    order.Value.Status = status;

                    switch (order.Value.Status)
                    {
                        case OrderStatus.Paid:
                            order.Value.OrderTime = DateTime.Now;
                            break;
                        case OrderStatus.InRealisation:
                            order.Value.PackedTime = DateTime.Now;
                            break;
                        case OrderStatus.InDelivery:
                            order.Value.SendTime = DateTime.Now;
                            break;
                        case OrderStatus.Delivered:
                            order.Value.DeliveredTime = DateTime.Now;
                            break;
                    }

                    break;
                default: return new BadRequestResult();
            }

            return new OkResult();
        }
        public bool IsOrderStatusInRange(OrderStatus status)
        {
            return Enum.IsDefined(typeof(OrderStatus), status);
        }
    }
}

