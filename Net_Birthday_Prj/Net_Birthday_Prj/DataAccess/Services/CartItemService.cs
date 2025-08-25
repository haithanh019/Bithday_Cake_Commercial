using AutoMapper;
using BusinessLogic.DTOs.Carts;
using BusinessLogic.Entities;
using DataAccess.Services.Interfaces;
using DataAccess.UnitOfWork;
using Ultitity.Exceptions;

namespace DataAccess.Services.Implements
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CartItemService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartItemDTO>> GetAllAsync(int? cartId = null)
        {
            if (cartId.HasValue)
            {
                var filtered = await _uow.CartItemRepository.GetRangeAsync(
                    x => x.CartId == cartId.Value,
                    includeProperties: "Product",
                    sortBy: nameof(CartItem.CartItemId),
                    isAscending: true
                );
                return _mapper.Map<IEnumerable<CartItemDTO>>(filtered);
            }

            var all = await _uow.CartItemRepository.GetAllAsync(
                includeProperties: "Product",
                sortBy: nameof(CartItem.CartItemId),
                isAscending: true,
                pageNumber: 1,
                pageSize: 1000
            );
            return _mapper.Map<IEnumerable<CartItemDTO>>(all);
        }

        public async Task<CartItemDTO?> GetByIdAsync(int cartItemId)
        {
            var entity = await _uow.CartItemRepository.GetAsync(
                x => x.CartItemId == cartItemId,
                includeProperties: "Product"
            );
            return entity == null ? null : _mapper.Map<CartItemDTO>(entity);
        }

        public async Task<CartItemDTO> CreateAsync(CreateCartItemDTO dto)
        {
            if (dto.Quantity <= 0)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(dto.Quantity), new[] { "Quantity must be greater than 0." } }
                    });

            // Cart tồn tại?
            var cart = await _uow.ShoppingCartRepository.GetAsync(c => c.CartId == dto.CartId);
            if (cart == null)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(dto.CartId), new[] { "Cart not found." } }
                    });

            // Product tồn tại?
            var product = await _uow.ProductRepository.GetAsync(p => p.ProductId == dto.ProductId);
            if (product == null)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(dto.ProductId), new[] { "Product not found." } }
                    });

            // Nếu item đã tồn tại trong cart → cộng dồn
            var existed = await _uow.CartItemRepository.GetAsync(
                x => x.CartId == dto.CartId && x.ProductId == dto.ProductId
            );
            if (existed != null)
            {
                existed.Quantity += dto.Quantity;
                await _uow.SaveAsync();

                var saved = await _uow.CartItemRepository.GetAsync(
                    x => 
                        x.CartItemId == existed.CartItemId,
                        includeProperties: "Product"
                );
                return _mapper.Map<CartItemDTO>(saved!);
            }

            // Tạo mới
            var entity = _mapper.Map<CartItem>(dto);
            entity.UnitPrice = product.Price;   // snapshot giá tại thời điểm thêm
            await _uow.CartItemRepository.AddAsync(entity);
            await _uow.SaveAsync();

            var created = await _uow.CartItemRepository.GetAsync(
                x => 
                    x.CartItemId == entity.CartItemId,
                    includeProperties: "Product"
            );
            return _mapper.Map<CartItemDTO>(created!);
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(quantity), new[] { "Quantity must be greater than 0." } }
                    });

            var entity = await _uow.CartItemRepository.GetAsync(x => x.CartItemId == cartItemId);
            if (entity == null)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(cartItemId), new[] { "Cart item not found." } }
                    });

            entity.Quantity = quantity;
            await _uow.SaveAsync();
        }

        public async Task DeleteAsync(int cartItemId)
        {
            var entity = await _uow.CartItemRepository.GetAsync(x => x.CartItemId == cartItemId);
            if (entity == null) return;

            _uow.CartItemRepository.Remove(entity);
            await _uow.SaveAsync();
        }
    }
}
