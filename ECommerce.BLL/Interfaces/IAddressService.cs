using ECommerce.DAL.Entities;

namespace ECommerce.BLL.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(string userId);
        Task<Address?> GetAddressByIdAsync(int id, string userId);
        Task CreateAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
        Task DeleteAddressAsync(int id, string userId);
    }
}
