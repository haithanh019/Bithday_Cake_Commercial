using AutoMapper;
using BusinessLogic.DTOs.CustomCakeOptions;
using BusinessLogic.Entities;
using DataAccess.Services.Interfaces;
using DataAccess.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultitity.Exceptions;

namespace DataAccess.Services
{
    public class CustomCakeOptionService : ICustomCakeOptionService
    {
        private static readonly HashSet<string> AllowedTypes =
            new(new[] { "Size", "Flavor", "Decoration" }, StringComparer.OrdinalIgnoreCase);

        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CustomCakeOptionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomCakeOptionDTO>> GetAllAsync(string? optionType = null)
        {
            if (!string.IsNullOrWhiteSpace(optionType))
            {
                var filtered = await _uow.CustomCakeOptionRepository.GetRangeAsync(
                    x => x.OptionType.ToLower() == optionType.ToLower(),
                    sortBy: nameof(CustomCakeOption.OptionId),
                    isAscending: true
                );
                return _mapper.Map<IEnumerable<CustomCakeOptionDTO>>(filtered);
            }

            var all = await _uow.CustomCakeOptionRepository.GetAllAsync(
                sortBy: nameof(CustomCakeOption.OptionId),
                isAscending: true,
                pageNumber: 1,
                pageSize: 1000
            );
            return _mapper.Map<IEnumerable<CustomCakeOptionDTO>>(all);
        }

        public async Task<CustomCakeOptionDTO?> GetByIdAsync(int optionId)
        {
            var entity = await _uow.CustomCakeOptionRepository.GetAsync(x => x.OptionId == optionId);
            return entity == null ? null : _mapper.Map<CustomCakeOptionDTO>(entity);
        }

        public async Task<CustomCakeOptionDTO> CreateAsync(CreateCustomCakeOptionDTO dto)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(dto.OptionType) || !AllowedTypes.Contains(dto.OptionType))
                errors[nameof(dto.OptionType)] = new[] { "OptionType must be one of: Size, Flavor, Decoration." };

            if (string.IsNullOrWhiteSpace(dto.Name))
                errors[nameof(dto.Name)] = new[] { "Name is required." };

            if (dto.ExtraPrice < 0m)
                errors[nameof(dto.ExtraPrice)] = new[] { "ExtraPrice cannot be negative." };

            if (errors.Any()) throw new CustomValidationException(errors);

            // Unique (OptionType, Name)
            var dup = await _uow.CustomCakeOptionRepository.GetAsync(
                x => x.OptionType.ToLower() == dto.OptionType.ToLower() && x.Name.ToLower() == dto.Name.ToLower()
            );
            if (dup != null)
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.Name), new[] { "An option with the same type and name already exists." } }
                });

            var entity = _mapper.Map<CustomCakeOption>(dto);
            await _uow.CustomCakeOptionRepository.AddAsync(entity);
            await _uow.SaveAsync();

            return _mapper.Map<CustomCakeOptionDTO>(entity);
        }

        public async Task UpdateAsync(UpdateCustomCakeOptionDTO dto)
        {
            var entity = await _uow.CustomCakeOptionRepository.GetAsync(x => x.OptionId == dto.OptionId);
            if (entity == null)
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.OptionId), new[] { "Option not found." } }
                });

            if (string.IsNullOrWhiteSpace(dto.OptionType) || !AllowedTypes.Contains(dto.OptionType))
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.OptionType), new[] { "OptionType must be one of: Size, Flavor, Decoration." } }
                });

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.Name), new[] { "Name is required." } }
                });

            if (dto.ExtraPrice < 0m)
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.ExtraPrice), new[] { "ExtraPrice cannot be negative." } }
                });

            // Check duplicate (excluding self)
            var dup = await _uow.CustomCakeOptionRepository.GetAsync(
                x => x.OptionType.ToLower() == dto.OptionType.ToLower()
                  && x.Name.ToLower() == dto.Name.ToLower()
                  && x.OptionId != dto.OptionId
            );
            if (dup != null)
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.Name), new[] { "An option with the same type and name already exists." } }
                });

            _mapper.Map(dto, entity);
            await _uow.SaveAsync();
        }

        public async Task DeleteAsync(int optionId)
        {
            var entity = await _uow.CustomCakeOptionRepository.GetAsync(x => x.OptionId == optionId);
            if (entity == null) return;

            _uow.CustomCakeOptionRepository.Remove(entity);
            await _uow.SaveAsync();
        }
    }
}
