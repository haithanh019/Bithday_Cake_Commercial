using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Deltas;
using Ultitity.Exceptions;
using DataAccess.Services.FacadeService;
using BusinessLogic.DTOs.Carts.CartItems;

namespace BirthdayCakeAPI.Controllers
{
    public class CartItemsController : ODataController
    {
        private readonly IFacadeService _facade;

        public CartItemsController(IFacadeService facade)
        {
            _facade = facade;
        }

        // GET /odata/CartItems
        // Có thể dùng $filter=CartId eq 1 để lọc theo Cart
        [EnableQuery(PageSize = 200)]
        public async Task<IActionResult> Get()
        {
            var data = await _facade.CartItemService.GetAllAsync();
            return Ok(data);
        }

        // GET /odata/CartItems(10)
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var item = await _facade.CartItemService.GetByIdAsync(key);
            return item == null ? NotFound() : Ok(item);
        }

        // POST /odata/CartItems
        // Body: { "CartId":1, "ProductId":5, "Quantity":2 }
        public async Task<IActionResult> Post([FromBody] CreateCartItemDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _facade.CartItemService.CreateAsync(dto);
                return Created(created);
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // PUT /odata/CartItems(10)
        // Chỉ cập nhật Quantity (theo đúng service UpdateQuantityAsync)
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] UpdateCartItemDTO dto)
        {
            if (key != dto.CartItemId) return BadRequest("Mismatched CartItemId.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _facade.CartItemService.UpdateQuantityAsync(key, dto.Quantity);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // PATCH /odata/CartItems(10)
        // Hỗ trợ cập nhật một phần (đặc biệt là Quantity)
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<UpdateCartItemDTO> delta)
        {
            if (delta is null) return BadRequest("Invalid payload.");

            // apply vào DTO tạm rồi gọi service cập nhật số lượng
            var temp = new UpdateCartItemDTO { CartItemId = key };
            delta.Patch(temp);

            try
            {
                await _facade.CartItemService.UpdateQuantityAsync(key, temp.Quantity);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // DELETE /odata/CartItems(10)
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            await _facade.CartItemService.DeleteAsync(key);
            return NoContent();
        }
    }
}
