using BusinessLogic.DTOs.Carts.CartItems;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DTOs.Carts
{
    public class ShoppingCartDTO
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
    }
}
