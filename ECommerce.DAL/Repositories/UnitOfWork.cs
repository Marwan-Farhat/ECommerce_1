using ECommerce.DAL.Context;
using ECommerce.DAL.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IRepository<Category>? _categories;
        private IRepository<Product>? _products;
        private IRepository<Order>? _orders;
        private IRepository<OrderItem>? _orderItems;
        private IRepository<Address>? _addresses;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<Category> Categories
            => _categories ??= new Repository<Category>(_context);

        public IRepository<Product> Products
            => _products ??= new Repository<Product>(_context);

        public IRepository<Order> Orders
            => _orders ??= new Repository<Order>(_context);

        public IRepository<OrderItem> OrderItems
            => _orderItems ??= new Repository<OrderItem>(_context);

        public IRepository<Address> Addresses
            => _addresses ??= new Repository<Address>(_context);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
