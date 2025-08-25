using BusinessLogic.DTOs.CustomCakeOptions;
using DataAccess.Services.FacadeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Ultitity.Exceptions;

namespace BirthdayCakeAPI.Controllers
{
    public class CustomCakeOptionsController : ODataController
    {
        private readonly IFacadeService _facade;
        public CustomCakeOptionsController(IFacadeService facade) => _facade = facade;

        // GET /odata/CustomCakeOptions
        // có thể lọc: ?$filter=OptionType eq 'Size'
        [EnableQuery(PageSize = 200)]
        public async Task<IActionResult> Get()
        {
            var data = await _facade.CustomCakeOptionService.GetAllAsync();
            return Ok(data);
        }

        // GET /odata/CustomCakeOptions(1)
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var item = await _facade.CustomCakeOptionService.GetByIdAsync(key);
            return item == null ? NotFound() : Ok(item);
        }

        // POST /odata/CustomCakeOptions
        public async Task<IActionResult> Post([FromBody] CreateCustomCakeOptionDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _facade.CustomCakeOptionService.CreateAsync(dto);
                return Created(created);
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // PUT /odata/CustomCakeOptions(1)
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] UpdateCustomCakeOptionDTO dto)
        {
            if (key != dto.OptionId) return BadRequest("Mismatched OptionId.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                await _facade.CustomCakeOptionService.UpdateAsync(dto);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // DELETE /odata/CustomCakeOptions(1)
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            await _facade.CustomCakeOptionService.DeleteAsync(key);
            return NoContent();
        }
    }
}
