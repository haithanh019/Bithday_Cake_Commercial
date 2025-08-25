using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DTOs.Orders.OrderItemDTOs
{
    public class OrderItemDTO
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // unit price snapshot
    }
}

