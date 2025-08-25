using BusinessLogic.DTOs.Carts;
namespace DataAccess.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemDTO>> GetAllAsync(int? cartId = null);
        Task<CartItemDTO?> GetByIdAsync(int cartItemId);
        Task<CartItemDTO> CreateAsync(CreateCartItemDTO dto);      // auto gộp nếu đã tồn tại
        Task UpdateQuantityAsync(int cartItemId, int quantity);     // chỉ cho phép đổi số lượng
        Task DeleteAsync(int cartItemId);
    }
}
