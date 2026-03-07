using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Entities;
using ECommerce.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(string userId, int shippingAddressId, List<CartItem> cartItems)
        {

                foreach (var item in cartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product == null || product.StockQuantity < item.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for product: {item.ProductName}");
                }

                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = shippingAddressId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = cartItems.Sum(i => i.SubTotal)
                };
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    await _unitOfWork.OrderItems.AddAsync(orderItem);

                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    product!.StockQuantity -= item.Quantity;
                    _unitOfWork.Products.Update(product);
                }

                await _unitOfWork.SaveChangesAsync();

                return order;
            
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
            => await _unitOfWork.Orders.Query()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        public async Task<PagedResult<Order>> GetUserOrdersPagedAsync(string userId, int page, int pageSize)
        {
            var query = _unitOfWork.Orders.Query()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Order>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId, string userId)
            => await _unitOfWork.Orders.Query()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
            => await _unitOfWork.Orders.Query()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task CancelOrderAsync(int orderId, string userId)
        {
                var order = await _unitOfWork.Orders.Query()
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order == null)
                    throw new InvalidOperationException("Order not found.");

                if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                    throw new InvalidOperationException("Cannot cancel a shipped or delivered order.");

                if (order.Status == OrderStatus.Cancelled)
                    throw new InvalidOperationException("Order is already cancelled.");

                foreach (var item in order.OrderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        _unitOfWork.Products.Update(product);
                    }
                }

                order.Status = OrderStatus.Cancelled;
                _unitOfWork.Orders.Update(order);

                await _unitOfWork.SaveChangesAsync();
        }
    }
}
