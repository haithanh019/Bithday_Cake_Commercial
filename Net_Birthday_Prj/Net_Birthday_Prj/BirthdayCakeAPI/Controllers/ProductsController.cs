using BusinessLogic.DTOs.Products;
using DataAccess.Services.FacadeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Ultitity.Exceptions;

namespace BirthdayCakeAPI.Controllers
{
    public class ProductsController : ODataController
    {
        private readonly IFacadeService _facade;

        public ProductsController(IFacadeService facadeService)
        {
            _facade = facadeService;
        }

        [EnableQuery(PageSize = 100)]
        public async Task<IActionResult> Get()
        {
            var data = await _facade.ProductService.GetAllAsync();
            return Ok(data);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var item = await _facade.ProductService.GetByIdAsync(key);
            return item == null ? NotFound() : Ok(item);
        }

        public async Task<IActionResult> Post([FromBody] CreateProductDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _facade.ProductService.CreateAsync(dto);
                return Created(created);
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { ex.Message, ex.Errors });
            }
        }

        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] UpdateProductDTO dto)
        {
            if (key != dto.ProductId) return BadRequest("Mismatched ProductId");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _facade.ProductService.UpdateAsync(dto);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { ex.Message, ex.Errors });
            }
        }

        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<UpdateProductDTO> delta)
        {
            if (delta == null) return BadRequest("Invalid payload.");

            var current = await _facade.ProductService.GetByIdAsync(key);
            if (current == null) return NotFound();

            var dto = new UpdateProductDTO
            {
                ProductId = current.ProductId,
                CategoryId = current.CategoryId,
                Name = current.Name,
                Description = current.Description,
                Price = current.Price,
                ImageUrl = current.ImageUrl,
                IsAvailable = current.IsAvailable
            };
            delta.Patch(dto);

            try
            {
                await _facade.ProductService.UpdateAsync(dto);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { ex.Message, ex.Errors });
            }
        }

        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            try
            {
                await _facade.ProductService.DeleteAsync(key);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { ex.Message, ex.Errors });
            }
        }
    }
}
