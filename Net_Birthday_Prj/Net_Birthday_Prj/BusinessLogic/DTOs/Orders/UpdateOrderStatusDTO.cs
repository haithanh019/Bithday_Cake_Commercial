namespace BusinessLogic.DTOs.Orders
{
    public class UpdateOrderStatusDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
