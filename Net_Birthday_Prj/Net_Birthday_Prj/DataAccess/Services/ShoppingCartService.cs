using AutoMapper;
using BusinessLogic.DTOs.Carts;
using BusinessLogic.DTOs.Carts.CartItems;
using BusinessLogic.Entities;
using DataAccess.Services.Interfaces;
using DataAccess.UnitOfWork;
using Ultitity.Exceptions;

namespace DataAccess.Services.Implements
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ShoppingCartDTO> GetOrCreateAsync(int userId)
        {
            var cart = await _unitOfWork.ShoppingCartRepository
                .GetAsync(c => c.UserId == userId, includeProperties: "Items.Product");

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId, CreatedAt = DateTime.UtcNow };
                await _unitOfWork.ShoppingCartRepository.AddAsync(cart);
                await _unitOfWork.SaveAsync();

                // reload với include để trả DTO đầy đủ
                cart = await _unitOfWork.ShoppingCartRepository
                    .GetAsync(c => c.CartId == cart.CartId, includeProperties: "Items.Product");
            }

            return _mapper.Map<ShoppingCartDTO>(cart);
        }

        public async Task<ShoppingCartDTO?> GetByIdAsync(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCartRepository
                .GetAsync(c => c.CartId == cartId, includeProperties: "Items.Product");
            return cart == null ? null : _mapper.Map<ShoppingCartDTO>(cart);
        }

        public async Task<IEnumerable<CartItemDTO>> GetItemsAsync(int cartId)
        {
            var items = await _unitOfWork.CartItemRepository.GetRangeAsync(
                ci => ci.CartId == cartId,
                includeProperties: "Product",
                sortBy: nameof(CartItem.CartItemId)
            );
            return _mapper.Map<IEnumerable<CartItemDTO>>(items);
        }

        public async Task<CartItemDTO> AddItemAsync(CreateCartItemDTO dto)
        {
            if (dto.Quantity <= 0)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(dto.Quantity), new[] { "Quantity must be > 0." } }
                    });

            var cart = await _unitOfWork.ShoppingCartRepository.GetAsync(c => c.CartId == dto.CartId);
            if (cart == null)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(dto.CartId), new[] { "Cart not found." } }
                    });

            var product = await _unitOfWork.ProductRepository.GetAsync(p => p.ProductId == dto.ProductId);
            if (product == null)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(dto.ProductId), new[] { "Product not found." } }
                    });

            // Cộng dồn nếu đã có
            var existed = await _unitOfWork.CartItemRepository.GetAsync(
                ci => ci.CartId == dto.CartId && ci.ProductId == dto.ProductId
            );
            if (existed != null)
            {
                existed.Quantity += dto.Quantity;
                await _unitOfWork.SaveAsync();

                var existedWithProduct = await _unitOfWork.CartItemRepository.GetAsync(
                    ci => ci.CartItemId == existed.CartItemId,
                    includeProperties: "Product"
                );
                return _mapper.Map<CartItemDTO>(existedWithProduct!);
            }

            var item = _mapper.Map<CartItem>(dto);
            item.UnitPrice = product.Price; // chốt giá tại thời điểm thêm

            await _unitOfWork.CartItemRepository.AddAsync(item);
            await _unitOfWork.SaveAsync();

            var saved = await _unitOfWork.CartItemRepository.GetAsync(
                ci => ci.CartItemId == item.CartItemId,
                includeProperties: "Product"
            );
            return _mapper.Map<CartItemDTO>(saved!);
        }

        public async Task UpdateItemQuantityAsync(int cartItemId, int quantity)
        {
            if (quantity <= 0)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(quantity), new[] { "Quantity must be > 0." } }
                    });

            var item = await _unitOfWork.CartItemRepository.GetAsync(ci => ci.CartItemId == cartItemId);
            if (item == null)
                throw new CustomValidationException(
                    new Dictionary<string, string[]>
                    {
                        { nameof(cartItemId), new[] { "Cart item not found." } }
                    });

            item.Quantity = quantity;
            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _unitOfWork.CartItemRepository.GetAsync(ci => ci.CartItemId == cartItemId);
            if (item == null) return;

            _unitOfWork.CartItemRepository.Remove(item);
            await _unitOfWork.SaveAsync();
        }

        public async Task ClearAsync(int cartId)
        {
            var items = await _unitOfWork.CartItemRepository.GetRangeAsync(ci => ci.CartId == cartId);
            if (items.Any())
            {
                _unitOfWork.CartItemRepository.RemoveRange(items);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
