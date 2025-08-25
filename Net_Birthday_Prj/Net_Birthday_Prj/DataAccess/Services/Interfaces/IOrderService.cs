
using BusinessLogic.DTOs.Orders;

namespace DataAccess.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllAsync();
        Task<OrderDTO?> GetByIdAsync(int orderId);

        // Checkout: tạo Order từ Cart, snapshot giá từ CartItem.UnitPrice
        Task<OrderDTO> CreateFromCartAsync(CreateOrderFromCartDTO dto);

        Task UpdateStatusAsync(UpdateOrderStatusDTO dto);
        Task DeleteAsync(int orderId); // tuỳ chính sách (có thể không xoá mà chỉ Cancel)
    }
}
