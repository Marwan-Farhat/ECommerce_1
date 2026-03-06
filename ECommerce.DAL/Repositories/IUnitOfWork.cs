using ECommerce.DAL.Entities;

namespace ECommerce.DAL.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Category> Categories { get; }
        IRepository<Product> Products { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<Address> Addresses { get; }

        Task<int> SaveChangesAsync();
    }
}
