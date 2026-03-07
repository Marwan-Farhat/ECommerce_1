using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Entities;
using ECommerce.PL.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var vm = orders.Select(o => new AdminOrderListVM
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                CustomerName = $"{o.User.FirstName} {o.User.LastName}".Trim(),
                CustomerEmail = o.User.Email ?? "",
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
            });
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();

            var vm = new OrderDetailsVM
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                ShippingAddress = new AddressVM
                {
                    FullName = order.ShippingAddress.FullName,
                    Street = order.ShippingAddress.Street,
                    City = order.ShippingAddress.City,
                    State = order.ShippingAddress.State,
                    PostalCode = order.ShippingAddress.PostalCode,
                    Country = order.ShippingAddress.Country
                },
                Items = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            };

            ViewBag.Statuses = Enum.GetValues<OrderStatus>();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(UpdateOrderStatusVM vm)
        {
            await _orderService.UpdateOrderStatusAsync(vm.OrderId, vm.NewStatus);
            TempData["Success"] = "Order status updated.";
            return RedirectToAction(nameof(Details), new { id = vm.OrderId });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var order = await _orderService.GetAllOrdersAsync();
                var o = order.FirstOrDefault(x => x.Id == id);

                if (o != null)
                    await _orderService.CancelOrderAsync(id, o.UserId);

                TempData["Success"] = $"Order #{id} has been cancelled.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
