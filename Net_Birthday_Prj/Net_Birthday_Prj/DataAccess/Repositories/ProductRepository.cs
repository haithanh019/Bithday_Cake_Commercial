using BusinessLogic.Entities;
using DataAccess.Data;
using DataAccess.Repositories.Interfaces;
namespace DataAccess.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}