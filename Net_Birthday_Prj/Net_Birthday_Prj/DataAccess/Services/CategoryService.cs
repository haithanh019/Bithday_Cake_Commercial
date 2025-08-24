using AutoMapper;
using BusinessLogic.DTOs.Categories;
using BusinessLogic.Entities;
using BusinessLogic.Services.Interfaces;
using DataAccess.UnitOfWork;
using Ultitity.Exceptions;
namespace BusinessLogic.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _unitOfWork = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var entities = await _unitOfWork.CategoryRepository
                .GetAllAsync(sortBy: nameof(Category.CategoryId), isAscending: true);
            return _mapper.Map<IEnumerable<CategoryDTO>>(entities);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int idCategory)
        {
            var entity = await _unitOfWork.CategoryRepository
                .GetAsync(x => x.CategoryId == idCategory);
            return entity == null ? null : _mapper.Map<CategoryDTO>(entity);
        }

        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO createCategoryDTO)
        {
            // Validate: name required + tránh trùng tên (case-insensitive)
            var name = (createCategoryDTO.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(createCategoryDTO.Name), new[] { "Name is required." } }
                });
            }

            var existed = await _unitOfWork.CategoryRepository
                .GetAsync(x => x.Name.ToLower() == name.ToLower());

            if (existed != null)
            {
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(createCategoryDTO.Name), new[] { "Category name already exists." } }
                });
            }

            var entity = _mapper.Map<Category>(createCategoryDTO);
            await _unitOfWork.CategoryRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<CategoryDTO>(entity);
        }

        public async Task UpdateAsync(UpdateCategoryDTO updateCategoryDTO)
        {
            var entity = await _unitOfWork.CategoryRepository
                .GetAsync(x => x.CategoryId == updateCategoryDTO.CategoryId);

            if (entity == null)
            {
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(updateCategoryDTO.CategoryId), new[] { "Category not found." } }
                });
            }

            // Tránh trùng tên với category khác
            var name = (updateCategoryDTO.Name ?? string.Empty).Trim();
            var dup = await _unitOfWork.CategoryRepository.GetAsync(
                x => x.Name.ToLower() == name.ToLower() && x.CategoryId != updateCategoryDTO.CategoryId
            );
            if (dup != null)
            {
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(updateCategoryDTO.Name), new[] { "Category name already exists." } }
                });
            }

            _mapper.Map(updateCategoryDTO, entity); // map full
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int idCategory)
        {
            // Nếu Category đang có Product, FK đang để Restrict => nên chặn trước
            var entity = await _unitOfWork.CategoryRepository
                .GetAsync(x => x.CategoryId == idCategory, includeProperties: "Products");

            if (entity == null)
                return;

            if (entity.Products != null && entity.Products.Any())
            {
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { "Products", new[] { "Cannot delete a category that has products." } }
                });
            }

            _unitOfWork.CategoryRepository.Remove(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
