using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Entities;
using ECommerce.PL.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.PL.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var vm = categories.Select(c => new CategoryFormVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive
            });
            return View(vm);
        }

        public IActionResult Create() => View(new CategoryFormVM());

        [HttpPost]
        public async Task<IActionResult> Create(CategoryFormVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            await _categoryService.CreateCategoryAsync(new Category
            {
                Name = vm.Name,
                Description = vm.Description,
                IsActive = vm.IsActive
            });

            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            return View(new CategoryFormVM
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryFormVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var category = await _categoryService.GetCategoryByIdAsync(vm.Id);
            if (category == null) return NotFound();

            category.Name = vm.Name;
            category.Description = vm.Description;
            category.IsActive = vm.IsActive;

            await _categoryService.UpdateCategoryAsync(category);
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            TempData["Success"] = "Category deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
