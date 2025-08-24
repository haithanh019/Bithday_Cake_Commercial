using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DTOs.Categories
{
    public class CategoryDTO
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}