using BusinessLogic.DTOs.Categories;
namespace BusinessLogic.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int idCategory);
        Task<CategoryDTO> CreateAsync(CreateCategoryDTO createCategoryDTO);
        Task UpdateAsync(UpdateCategoryDTO updateCategoryDTO);
        Task DeleteAsync(int idCategory);
    }
}
