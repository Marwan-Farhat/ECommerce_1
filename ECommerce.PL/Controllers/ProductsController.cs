using ECommerce.BLL.Interfaces;
using ECommerce.PL.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.PL.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? q, string? sort, int page = 1)
        {
            var filters = new ProductFilterParams
            {
                CategoryId = categoryId,
                Search = q,
                Sort = sort,
                Page = page,
                PageSize = 12
            };

            var result = await _productService.GetProductsAsync(filters);
            var categories = await _categoryService.GetActiveCategoriesAsync();

            var vm = new ProductListVM
            {
                Products = result.Items.Select(p => new ProductListItemVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    StockQuantity = p.StockQuantity,
                    CategoryName = p.Category?.Name ?? ""
                }),
                Categories = categories.Select(c => new CategoryVM
                {
                    Id = c.Id,
                    Name = c.Name
                }),
                CategoryId = categoryId,
                Search = q,
                Sort = sort,
                CurrentPage = result.Page,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount,
                HasPrev = result.HasPrev,
                HasNext = result.HasNext
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var vm = new ProductDetailsVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                CategoryName = product.Category?.Name ?? "",
                CategoryId = product.CategoryId
            };

            return View(vm);
        }
    }
}
