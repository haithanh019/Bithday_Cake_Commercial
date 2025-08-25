using BusinessLogic.Entities;
using DataAccess.Data;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext db) : base(db) { }
    }
}

