using ECommerce.DAL.Entities;

namespace ECommerce.PL.Models.ViewModels
{
    public class OrderListItemVM
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }

    public class OrderDetailsVM
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public AddressVM ShippingAddress { get; set; } = new();
        public List<OrderItemVM> Items { get; set; } = new();
    }

    public class OrderItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
        public string? ImageUrl { get; set; }
    }

    // Admin ViewModels
    public class AdminOrderListVM
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }

    public class UpdateOrderStatusVM
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
