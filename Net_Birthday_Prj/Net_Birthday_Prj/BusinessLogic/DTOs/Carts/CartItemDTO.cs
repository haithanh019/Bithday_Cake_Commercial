using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DTOs.Carts
{
    public class CartItemDTO
    {
        [Key]
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
