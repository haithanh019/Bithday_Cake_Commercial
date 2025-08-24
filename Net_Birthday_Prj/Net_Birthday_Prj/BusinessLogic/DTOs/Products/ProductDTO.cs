using System.ComponentModel.DataAnnotations;
namespace BusinessLogic.DTOs.Products
{
    public class ProductDTO
    {
        [Key]
        public int ProductId { get; set; }           // OData sẽ lấy đây làm key
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }    // tiện cho view
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
