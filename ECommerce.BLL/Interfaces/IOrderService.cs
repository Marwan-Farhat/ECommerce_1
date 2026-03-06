using ECommerce.DAL.Entities;

namespace ECommerce.BLL.Interfaces
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }

    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, int shippingAddressId, List<CartItem> cartItems);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderDetailsAsync(int orderId, string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
