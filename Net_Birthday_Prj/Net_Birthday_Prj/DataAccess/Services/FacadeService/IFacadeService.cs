using DataAccess.Services.Interfaces;

namespace DataAccess.Services.FacadeService
{
    public interface IFacadeService
    {
        ICategoryService CategoryService { get; }
        IProductService ProductService { get; }
        IShoppingCartService ShoppingCartService { get; }
        ICartItemService CartItemService { get; }
    }
}
