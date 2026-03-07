using ECommerce.BLL.Interfaces;
using ECommerce.BLL.Services;
using ECommerce.DAL.Entities;
using ECommerce.PL.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.PL.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            IOrderService orderService,
            IAddressService addressService,
            ICartService cartService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _addressService = addressService;
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 5;
            var userId = _userManager.GetUserId(User)!;

            var result = await _orderService.GetUserOrdersPagedAsync(userId, page, pageSize);

            var vm = new OrderListPagedVM
            {
                Orders = result.Items.Select(o => new OrderListItemVM
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
                }),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
                TotalCount = result.TotalCount
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cartItems = _cartService.GetCart(HttpContext.Session);
            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            var userId = _userManager.GetUserId(User)!;
            var addresses = await _addressService.GetUserAddressesAsync(userId);

            var vm = new CheckoutVM
            {
                Cart = new CartVM
                {
                    Items = cartItems.Select(c => new CartItemVM
                    {
                        ProductId = c.ProductId,
                        ProductName = c.ProductName,
                        UnitPrice = c.UnitPrice,
                        Quantity = c.Quantity,
                        ImageUrl = c.ImageUrl
                    }).ToList()
                },
                SavedAddresses = addresses.Select(a => new AddressVM
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Street = a.Street,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    Phone = a.Phone,
                    IsDefault = a.IsDefault
                })
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            var userId = _userManager.GetUserId(User)!;
            var cartItems = _cartService.GetCart(HttpContext.Session);

            if (!cartItems.Any())
                return RedirectToAction("Index", "Cart");

            int shippingAddressId;

            try
            {
                if (model.UseNewAddress)
                {
                    // Validate the new address
                    if (!ModelState.IsValid)
                    {
                        // Reload addresses for redisplay
                        var addrs = await _addressService.GetUserAddressesAsync(userId);
                        model.SavedAddresses = addrs.Select(a => new AddressVM { Id = a.Id, FullName = a.FullName, Street = a.Street, City = a.City, State = a.State, PostalCode = a.PostalCode, Country = a.Country });
                        model.Cart = BuildCartVM(cartItems);
                        return View(model);
                    }

                    var newAddr = new Address
                    {
                        UserId = userId,
                        FullName = model.NewAddress.FullName,
                        Street = model.NewAddress.Street,
                        City = model.NewAddress.City,
                        State = model.NewAddress.State,
                        PostalCode = model.NewAddress.PostalCode,
                        Country = model.NewAddress.Country,
                        Phone = model.NewAddress.Phone
                    };
                    await _addressService.CreateAddressAsync(newAddr);
                    shippingAddressId = newAddr.Id;
                }
                else
                {
                    if (!model.SelectedAddressId.HasValue)
                    {
                        ModelState.AddModelError("", "Please select a shipping address.");
                        var addrs = await _addressService.GetUserAddressesAsync(userId);
                        model.SavedAddresses = addrs.Select(a => new AddressVM { Id = a.Id, FullName = a.FullName, Street = a.Street });
                        model.Cart = BuildCartVM(cartItems);
                        return View(model);
                    }
                    shippingAddressId = model.SelectedAddressId.Value;
                }

                var order = await _orderService.CreateOrderAsync(userId, shippingAddressId, cartItems);
                _cartService.ClearCart(HttpContext.Session);

                TempData["Success"] = $"Order #{order.Id} placed successfully!";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var addrs = await _addressService.GetUserAddressesAsync(userId);
                model.SavedAddresses = addrs.Select(a => new AddressVM { Id = a.Id, FullName = a.FullName, Street = a.Street });
                model.Cart = BuildCartVM(cartItems);
                return View(model);
            }
        }  

        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var order = await _orderService.GetOrderDetailsAsync(id, userId);
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
                    Country = order.ShippingAddress.Country,
                    Phone = order.ShippingAddress.Phone
                },
                Items = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    ImageUrl = oi.Product.ImageUrl
                }).ToList()
            };

            return View(vm);
        }

        private CartVM BuildCartVM(List<CartItem> cartItems) => new CartVM
        {
            Items = cartItems.Select(c => new CartItemVM
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                UnitPrice = c.UnitPrice,
                Quantity = c.Quantity
            }).ToList()
        };

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            try
            {
                await _orderService.CancelOrderAsync(id, userId);
                TempData["Success"] = $"Order #{id} has been cancelled successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
