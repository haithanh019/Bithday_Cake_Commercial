using AutoMapper;
using BusinessLogic.DTOs.Orders;
using BusinessLogic.Entities;
using BusinessLogic.Enums;
using DataAccess.Services.Interfaces;
using DataAccess.UnitOfWork;
using Ultitity.Exceptions;

namespace DataAccess.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllAsync()
        {
            var entities = await _uow.OrderRepository
                .GetAllAsync(includeProperties: "Items", sortBy: nameof(Order.OrderId));
            return _mapper.Map<IEnumerable<OrderDTO>>(entities);
        }

        public async Task<OrderDTO?> GetByIdAsync(int orderId)
        {
            var entity = await _uow.OrderRepository
                .GetAsync(o => o.OrderId == orderId, includeProperties: "Items");
            return entity == null ? null : _mapper.Map<OrderDTO>(entity);
        }

        public async Task<OrderDTO> CreateFromCartAsync(CreateOrderFromCartDTO dto)
        {
            // Load cart + items + product
            var cart = await _uow.ShoppingCartRepository
                .GetAsync(c => c.CartId == dto.CartId, includeProperties: "Items,Items.Product");
            if (cart == null)
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.CartId), new[] { "Cart not found." } }
                });

            if (cart.Items == null || !cart.Items.Any())
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { "Items", new[] { "Cart is empty." } }
                });

            // Tạo order
            var order = new Order
            {
                UserId = cart.UserId,
                CartId = cart.CartId,
                DeliveryAddress = dto.DeliveryAddress,
                Status = OrderStatus.Pending.ToString(),   // dùng enum → string
                CreatedAt = DateTime.UtcNow
            };

            // Snapshot order items từ cart items
            decimal total = 0m;
            foreach (var ci in cart.Items)
            {
                var oi = new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.UnitPrice > 0 ? ci.UnitPrice : ci.Product.Price // fallback
                };
                order.Items.Add(oi);
                total += oi.Price * oi.Quantity;
            }
            order.TotalAmount = total;

            // Lưu
            await _uow.OrderRepository.AddAsync(order);

            // (tuỳ) Clear cart sau khi tạo order
            var toRemove = await _uow.CartItemRepository.GetRangeAsync(x => x.CartId == cart.CartId);
            if (toRemove.Any()) _uow.CartItemRepository.RemoveRange(toRemove);

            await _uow.SaveAsync();

            // Load lại kèm Items để map DTO
            var saved = await _uow.OrderRepository
                .GetAsync(o => o.OrderId == order.OrderId, includeProperties: "Items");
            return _mapper.Map<OrderDTO>(saved!);
        }

        public async Task UpdateStatusAsync(UpdateOrderStatusDTO dto)
        {
            var entity = await _uow.OrderRepository.GetAsync(o => o.OrderId == dto.OrderId);
            if (entity == null)
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { nameof(dto.OrderId), new[] { "Order not found." } }
                });

            entity.Status = dto.Status;
            await _uow.SaveAsync();
        }

        public async Task DeleteAsync(int orderId)
        {
            var entity = await _uow.OrderRepository.GetAsync(o => o.OrderId == orderId, "Items");
            if (entity == null) return;

            // tuỳ chính sách: nếu đã Paid thì không cho delete
            if (string.Equals(entity.Status, OrderStatus.Confirmed.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new CustomValidationException(new Dictionary<string, string[]>
                {
                    { "Status", new[] { "Cannot delete a confirmed order." } }
                });

            // EF sẽ cascade xoá Items
            _uow.OrderRepository.Remove(entity);
            await _uow.SaveAsync();
        }
    }
}
