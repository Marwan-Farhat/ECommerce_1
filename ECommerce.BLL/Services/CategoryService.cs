using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Entities;
using ECommerce.DAL.Repositories;

namespace ECommerce.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _unitOfWork.Categories.FindAsync(c => c.IsActive);

        public async Task<Category?> GetCategoryByIdAsync(int id)
            => await _unitOfWork.Categories.GetByIdAsync(id);

        public async Task CreateCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category != null)
            {
                category.IsActive = false;
                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
