using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Entities;
using ECommerce.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<Product>> GetProductsAsync(ProductFilterParams filters)
        {
            var query = _unitOfWork.Products.Query()
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.Category.IsActive);

            // Filter by category
            if (filters.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filters.CategoryId.Value);

            // Search
            if (!string.IsNullOrWhiteSpace(filters.Search))
                query = query.Where(p => p.Name.Contains(filters.Search) || p.Description.Contains(filters.Search));

            // Sort
            query = filters.Sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filters.Page,
                PageSize = filters.PageSize
            };
        }
        public async Task<IEnumerable<Product>> GetAllProductsForAdminAsync()
        {
            return await _unitOfWork.Products.Query()
                .Include(p => p.Category)
                .OrderBy(p => p.Id)
                .ToListAsync();  
        }
        public async Task<Product?> GetProductByIdAsync(int id)
            => await _unitOfWork.Products.Query()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
            => await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId && p.IsActive);

        public async Task CreateProductAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                product.IsActive = false; // Soft delete
                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> DecreaseStockAsync(int productId, int quantity)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null || product.StockQuantity < quantity)
                return false;

            product.StockQuantity -= quantity;
            _unitOfWork.Products.Update(product);
            return true;
        }
    }
}
