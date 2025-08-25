using BusinessLogic.DTOs.Carts;
using BusinessLogic.DTOs.Carts.CartItems;

namespace DataAccess.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDTO> GetOrCreateAsync(int userId);
        Task<ShoppingCartDTO?> GetByIdAsync(int cartId);
        Task<IEnumerable<CartItemDTO>> GetItemsAsync(int cartId);
        Task<CartItemDTO> AddItemAsync(CreateCartItemDTO dto);
        Task UpdateItemQuantityAsync(int cartItemId, int quantity);
        Task RemoveItemAsync(int cartItemId);
        Task ClearAsync(int cartId);
    }
}