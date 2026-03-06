namespace ECommerce.PL.Models.ViewModels
{
    public class ProductListItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class ProductListVM
    {
        public IEnumerable<ProductListItemVM> Products { get; set; } = new List<ProductListItemVM>();
        public IEnumerable<CategoryVM> Categories { get; set; } = new List<CategoryVM>();

        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrev { get; set; }
        public bool HasNext { get; set; }
    }

    public class ProductDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public bool InStock => StockQuantity > 0;
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }

    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }
}
