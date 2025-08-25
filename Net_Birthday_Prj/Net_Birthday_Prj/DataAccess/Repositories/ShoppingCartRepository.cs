using BusinessLogic.Entities;
using DataAccess.Data;
using DataAccess.Repositories.Interfaces;
namespace DataAccess.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext db) : base(db) { }
    }
}
