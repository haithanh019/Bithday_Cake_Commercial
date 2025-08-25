using BusinessLogic.DTOs.Orders.OrderItemDTOs;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DTOs.Orders
{
    public class OrderDTO
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? DeliveryAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    }
}

