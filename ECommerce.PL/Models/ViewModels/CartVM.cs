using System.ComponentModel.DataAnnotations;

namespace ECommerce.PL.Models.ViewModels
{
    public class CartItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }

    public class CartVM
    {
        public List<CartItemVM> Items { get; set; } = new();
        public decimal Total => Items.Sum(i => i.SubTotal);
        public int ItemCount => Items.Sum(i => i.Quantity);
        public bool IsEmpty => !Items.Any();
    }

    public class AddressVM
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public bool IsDefault { get; set; }
        public string DisplayText => $"{FullName}, {Street}, {City}, {State} {PostalCode}, {Country}";
    }

    public class NewAddressVM
    {
        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(250)]
        public string Street { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }
    }

    public class CheckoutVM
    {
        public CartVM Cart { get; set; } = new();
        public IEnumerable<AddressVM> SavedAddresses { get; set; } = new List<AddressVM>();
        public int? SelectedAddressId { get; set; }
        public NewAddressVM NewAddress { get; set; } = new();
        public bool UseNewAddress { get; set; } = false;
        public string? Notes { get; set; }
    }
}
