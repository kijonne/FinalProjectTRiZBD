using OnlineShoeStoreLibrary.DTOs;
using OnlineShoeStoreLibrary.Models;

namespace OnlineShoeStoreLibrary.Extensions
{
    public static class OrderExtensions
    {
        public static OrderDto ToDto(this Order order)
        {
            var composition = string.Join(", ", order.OrderItems
                .Select(oi => $"{oi.Product?.Article ?? "Товар"} - {oi.Quantity} шт."));

            var total = order.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price);

            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate,
                ReceiveCode = order.ReceiveCode,
                IsFinished = order.IsFinished,
                TotalAmount = total,
                UserLogin = order.User?.Login ?? "",
            };
        }

        public static IEnumerable<OrderDto> ToDtos(this IEnumerable<Order> orders)
        {
            return orders.Select(o => o.ToDto());
        }

        public static List<OrderDto> ToDtos(this List<Order> orders)
        {
            return orders.Select(o => o.ToDto()).ToList();
        }
    }
}