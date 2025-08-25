using AutoMapper;
using BusinessLogic.DTOs.Products;
using BusinessLogic.Entities;
using DataAccess.Services.Interfaces;
using DataAccess.UnitOfWork;
using Ultitity.Exceptions;

namespace DataAccess.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork uow, IMapper mapper)
        {
            _unitOfWork = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var entities = await _unitOfWork.ProductRepository
                .GetAllAsync(includeProperties: "Category", sortBy: nameof(Product.ProductId));
            return _mapper.Map<IEnumerable<ProductDTO>>(entities);
        }

        public async Task<ProductDTO?> GetByIdAsync(int idProduct)
        {
            var entity = await _unitOfWork.ProductRepository
                .GetAsync(x => x.ProductId == idProduct, includeProperties: "Category");
            return entity == null ? null : _mapper.Map<ProductDTO>(entity);
        }

        public async Task<ProductDTO> CreateAsync(CreateProductDTO dto)
        {
            // Validate name required
            var name = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.Name), new[] { "Name is required." } }
                });
            }

            var entity = _mapper.Map<Product>(dto);
            await _unitOfWork.ProductRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<ProductDTO>(entity);
        }

        public async Task UpdateAsync(UpdateProductDTO dto)
        {
            var entity = await _unitOfWork.ProductRepository
                .GetAsync(x => x.ProductId == dto.ProductId);

            if (entity == null)
            {
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.ProductId), new[] { "Product not found." } }
                });
            }

            _mapper.Map(dto, entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int idProduct)
        {
            var entity = await _unitOfWork.ProductRepository.GetAsync(x => x.ProductId == idProduct);
            if (entity == null) return;

            _unitOfWork.ProductRepository.Remove(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
