using BusinessLogic.DTOs.Carts;
using DataAccess.Services.FacadeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BirthdayCakeAPI.Controllers
{

    public class ShoppingCartsController : ODataController
    {
        private readonly IFacadeService _facade;
        public ShoppingCartsController(IFacadeService facade) => _facade = facade;

        // GET /odata/ShoppingCarts?$expand=Items($expand=Product)
        [EnableQuery(PageSize = 50)]
        public async Task<IActionResult> Get() => Ok(await _facade.ShoppingCartService.GetOrCreateAsync(0)); // placeholder nếu cần

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var cart = await _facade.ShoppingCartService.GetByIdAsync(key);
            return cart == null ? NotFound() : Ok(cart);
        }

        // Tạo cart cho user (hoặc trả cart có sẵn)
        public async Task<IActionResult> Post([FromBody] CreateShoppingCartDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var cart = await _facade.ShoppingCartService.GetOrCreateAsync(dto.UserId);
            return Created(cart);
        }
    }
}
