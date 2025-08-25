namespace BusinessLogic.DTOs.Orders
{
    public class CreateOrderFromCartDTO
    {
        public int CartId { get; set; }
        public string? DeliveryAddress { get; set; }
    }

}

