using AutoMapper;
using BusinessLogic.DTOs.CustomCakeOptions;
using BusinessLogic.Entities;
using BusinessLogic.Enums;
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
                if (!TryParseOptionType(optionType, out var typeEnum))
                {
                    throw new CustomValidationException(new Dictionary<string, string[]>
            {
                { nameof(optionType), new[] { $"Invalid optionType. Allowed: {AllowedOptionTypesText()}." } }
            });
                }

                var filtered = await _uow.CustomCakeOptionRepository.GetRangeAsync(
                    x => x.OptionType == typeEnum,
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

            // Validate OptionType
            if (!TryParseOptionType(dto.OptionType, out var typeEnum))
                errors[nameof(dto.OptionType)] = new[] { $"OptionType must be one of: {AllowedOptionTypesText()}." };

            // Validate Name
            if (string.IsNullOrWhiteSpace(dto.Name))
                errors[nameof(dto.Name)] = new[] { "Name is required." };

            // Validate ExtraPrice
            if (dto.ExtraPrice < 0m)
                errors[nameof(dto.ExtraPrice)] = new[] { "ExtraPrice cannot be negative." };

            if (errors.Any()) throw new CustomValidationException(errors);

            // Unique (OptionType, Name) – so sánh enum trực tiếp, Name case-insensitive
            var dup = await _uow.CustomCakeOptionRepository.GetAsync(
                x => x.OptionType == typeEnum && x.Name.ToLower() == dto.Name.Trim().ToLower()
            );
            if (dup != null)
                throw new CustomValidationException(new Dictionary<string, string[]>
        {
            { nameof(dto.Name), new[] { "An option with the same type and name already exists." } }
        });

            // Map DTO -> Entity (vì DTO có OptionType string, set enum thủ công)
            var entity = new CustomCakeOption
            {
                Name = dto.Name.Trim(),
                ExtraPrice = dto.ExtraPrice,
                OptionType = typeEnum
            };

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

            // Check duplicate
            var normalizedName = dto.Name.Trim().ToLower();
            var dup = await _uow.CustomCakeOptionRepository.GetAsync(
                x => x.OptionType == dto.OptionType
                  && x.Name.ToLower() == normalizedName
                  && x.OptionId != dto.OptionId
            );
            if (dup != null)
                throw new CustomValidationException(new Dictionary<string, string[]>
        {
            { nameof(dto.Name), new[] { "An option with the same type and name already exists." } }
        });

            // Update entity
            entity.Name = dto.Name.Trim();
            entity.ExtraPrice = dto.ExtraPrice;
            entity.OptionType = dto.OptionType;

            await _uow.SaveAsync();
        }

        public async Task DeleteAsync(int optionId)
        {
            var entity = await _uow.CustomCakeOptionRepository.GetAsync(x => x.OptionId == optionId);
            if (entity == null) return;

            _uow.CustomCakeOptionRepository.Remove(entity);
            await _uow.SaveAsync();
        }

        private static bool TryParseOptionType(string? raw, out CustomOptionType value)
        {
            return Enum.TryParse<CustomOptionType>(raw?.Trim(), ignoreCase: true, out value);
        }

        private static string AllowedOptionTypesText()
        {
            return string.Join(", ", Enum.GetNames(typeof(CustomOptionType)));
        }

    }
}
