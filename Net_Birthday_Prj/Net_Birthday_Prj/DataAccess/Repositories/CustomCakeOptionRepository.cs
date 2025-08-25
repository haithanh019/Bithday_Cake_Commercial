using BusinessLogic.Entities;
using DataAccess.Data;
using DataAccess.Repositories.Interfaces;

namespace DataAccess.Repositories
{
    public class CustomCakeOptionRepository : Repository<CustomCakeOption>, ICustomCakeOptionRepository
    {
        public CustomCakeOptionRepository(ApplicationDbContext db) : base(db) { }
    }
}
