using ECommerce.BLL.Interfaces;
using ECommerce.BLL.Services;
using ECommerce.PL.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.PL.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            var cartItems = _cartService.GetCart(HttpContext.Session);
            var vm = new CartVM
            {
                Items = cartItems.Select(c => new CartItemVM
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    UnitPrice = c.UnitPrice,
                    Quantity = c.Quantity,
                    ImageUrl = c.ImageUrl
                }).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound();

            if (product.StockQuantity < quantity)
            {
                TempData["Error"] = "Not enough stock available.";
                return RedirectToAction("Details", "Products", new { id = productId });
            }

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity,
                ImageUrl = product.ImageUrl
            };

            _cartService.AddToCart(HttpContext.Session, cartItem);
            TempData["Success"] = $"{product.Name} added to cart!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Update(int productId, int quantity)
        {
            _cartService.UpdateCart(HttpContext.Session, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cartService.RemoveFromCart(HttpContext.Session, productId);
            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }
    }
}
