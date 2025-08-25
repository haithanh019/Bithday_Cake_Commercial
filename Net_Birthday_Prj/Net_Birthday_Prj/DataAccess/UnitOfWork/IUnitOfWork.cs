using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories.Interfaces.DataAccess.Repositories.Interfaces;

namespace DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        ICustomCakeOptionRepository CustomCakeOptionRepository { get; }
        Task SaveAsync();
    }
}
