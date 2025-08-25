using BusinessLogic.DTOs.CustomCakeOptions;

namespace DataAccess.Services.Interfaces
{
    public interface ICustomCakeOptionService
    {
        Task<IEnumerable<CustomCakeOptionDTO>> GetAllAsync(string? optionType = null);
        Task<CustomCakeOptionDTO?> GetByIdAsync(int optionId);
        Task<CustomCakeOptionDTO> CreateAsync(CreateCustomCakeOptionDTO dto);
        Task UpdateAsync(UpdateCustomCakeOptionDTO dto);
        Task DeleteAsync(int optionId);
    }
}

