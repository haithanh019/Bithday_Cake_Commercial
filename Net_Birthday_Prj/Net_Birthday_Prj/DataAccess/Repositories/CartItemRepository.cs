using BusinessLogic.Entities;
using DataAccess.Data;
using DataAccess.Repositories.Interfaces;
namespace DataAccess.Repositories
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext db) : base(db) { }
    }
}
