using BusinessLogic.DTOs.Products;

namespace BusinessLogic.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetByIdAsync(int idProduct);
        Task<ProductDTO> CreateAsync(CreateProductDTO dto);
        Task UpdateAsync(UpdateProductDTO dto);
        Task DeleteAsync(int idProduct);
    }
}
