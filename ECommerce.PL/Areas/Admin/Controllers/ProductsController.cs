using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Entities;
using ECommerce.PL.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsForAdminAsync(); 
            var vm = products.Select(p => new ProductListItemVM
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive
            });
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ProductFormVM
            {
                Categories = await GetCategorySelectList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await GetCategorySelectList();
                return View(vm);
            }

            await _productService.CreateProductAsync(new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                StockQuantity = vm.StockQuantity,
                ImageUrl = vm.ImageUrl,
                IsActive = vm.IsActive,
                CategoryId = vm.CategoryId
            });

            TempData["Success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductFormVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                Categories = await GetCategorySelectList(product.CategoryId)
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await GetCategorySelectList(vm.CategoryId);
                return View(vm);
            }

            var product = await _productService.GetProductByIdAsync(vm.Id);
            if (product == null) return NotFound();

            product.Name = vm.Name;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.StockQuantity = vm.StockQuantity;
            product.ImageUrl = vm.ImageUrl;
            product.IsActive = vm.IsActive;
            product.CategoryId = vm.CategoryId;

            await _productService.UpdateProductAsync(product);
            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["Success"] = "Product deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectList(int? selectedId = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == selectedId
            });
        }
    }
}
