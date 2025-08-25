using BusinessLogic.Entities;
using DataAccess.Data;
using DataAccess.Repositories.Interfaces.DataAccess.Repositories.Interfaces;
namespace DataAccess.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext db) : base(db) { }
    }
}

