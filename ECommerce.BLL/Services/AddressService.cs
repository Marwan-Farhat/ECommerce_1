using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Entities;
using ECommerce.DAL.Repositories;

namespace ECommerce.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Address>> GetUserAddressesAsync(string userId)
            => await _unitOfWork.Addresses.FindAsync(a => a.UserId == userId);

        public async Task<Address?> GetAddressByIdAsync(int id, string userId)
            => await _unitOfWork.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        public async Task CreateAddressAsync(Address address)
        {
            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAddressAsync(Address address)
        {
            _unitOfWork.Addresses.Update(address);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(int id, string userId)
        {
            var address = await GetAddressByIdAsync(id, userId);
            if (address != null)
            {
                _unitOfWork.Addresses.Remove(address);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
