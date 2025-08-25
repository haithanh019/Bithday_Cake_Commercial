// BirthdayCakeAPI/Controllers/OrdersController.cs
using BusinessLogic.DTOs.Orders;
using DataAccess.Services.FacadeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Ultitity.Exceptions;

namespace BirthdayCakeAPI.Controllers
{
    public class OrdersController : ODataController
    {
        private readonly IFacadeService _facade;
        public OrdersController(IFacadeService facade) => _facade = facade;

        // GET /odata/Orders?$expand=Items
        [EnableQuery(PageSize = 100)]
        public async Task<IActionResult> Get()
        {
            var data = await _facade.OrderService.GetAllAsync();
            return Ok(data);
        }

        // GET /odata/Orders(1)?$expand=Items
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var item = await _facade.OrderService.GetByIdAsync(key);
            return item == null ? NotFound() : Ok(item);
        }

        // POST /odata/Orders  (Checkout from Cart)
        // Body: { "CartId": 1, "DeliveryAddress": "123 Hai Bà Trưng" }
        public async Task<IActionResult> Post([FromBody] CreateOrderFromCartDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _facade.OrderService.CreateFromCartAsync(dto);
                return Created(created);
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // PUT /odata/Orders(1)  -> cập nhật Status
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] UpdateOrderStatusDTO dto)
        {
            if (key != dto.OrderId) return BadRequest("Mismatched OrderId");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _facade.OrderService.UpdateStatusAsync(dto);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // DELETE /odata/Orders(1)
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            try
            {
                await _facade.OrderService.DeleteAsync(key);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }
    }
}
