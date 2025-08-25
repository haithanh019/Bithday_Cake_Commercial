using BusinessLogic.DTOs.Users;
using DataAccess.Helper;
using DataAccess.Repositories.Interfaces;
using DataAccess.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequestDTO dto)
        {
            var user = await _userRepository.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var valid = await _userRepository.CheckPasswordAsync(user, dto.Password);
            if (!valid) return null;

            var roles = await _userRepository.GetRolesAsync(user);

            var token = JwtHelper.GenerateJwtToken(user.Id, user.Email!, _config);

            return new LoginResponse
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles
                }
            };
        }
    }
}

