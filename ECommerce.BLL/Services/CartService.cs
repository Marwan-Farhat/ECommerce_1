using ECommerce.BLL.Interfaces;
using ECommerce.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ECommerce.BLL.Services
{
    public interface ICartService
    {
        List<CartItem> GetCart(ISession session);
        void AddToCart(ISession session, CartItem item);
        void UpdateCart(ISession session, int productId, int quantity);
        void RemoveFromCart(ISession session, int productId);
        void ClearCart(ISession session);
        decimal GetCartTotal(ISession session);
    }

    public class CartService : ICartService
    {
        private const string CartKey = "ShoppingCart";
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CartItem> GetCart(ISession session)
        {
            var json = session.GetString(CartKey);
            return json == null
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        public void AddToCart(ISession session, CartItem item)
        {
            var cart = GetCart(session);
            var existing = cart.FirstOrDefault(c => c.ProductId == item.ProductId);
            if (existing != null)
                existing.Quantity += item.Quantity;
            else
                cart.Add(item);

            SaveCart(session, cart);
        }

        public void UpdateCart(ISession session, int productId, int quantity)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            SaveCart(session, cart);
        }

        public void RemoveFromCart(ISession session, int productId)
        {
            var cart = GetCart(session);
            cart.RemoveAll(c => c.ProductId == productId);
            SaveCart(session, cart);
        }

        public void ClearCart(ISession session)
            => session.Remove(CartKey);

        public decimal GetCartTotal(ISession session)
            => GetCart(session).Sum(c => c.SubTotal);

        private void SaveCart(ISession session, List<CartItem> cart)
            => session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }
}
