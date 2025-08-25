using BusinessLogic.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
            => await _userManager.CheckPasswordAsync(user, password);

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
            => await _userManager.GetRolesAsync(user);
    }
}
