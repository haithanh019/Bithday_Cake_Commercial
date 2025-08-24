using BusinessLogic.DTOs.Categories;
using BusinessLogic.Services.FacadeService;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Ultitity.Exceptions;

namespace BirthdayCakeAPI.Controllers
{
    public class CategoriesController : ODataController
    {
        private readonly IFacadeService _facade;

        public CategoriesController(IFacadeService facadeService)
        {
            _facade = facadeService;
        }

        // GET /odata/Categories
        [EnableQuery(PageSize = 100)]
        public async Task<IActionResult> Get()
        {
            var data = await _facade.CategoryService.GetAllAsync();
            return Ok(data);
        }

        // GET /odata/Categories(1)
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var item = await _facade.CategoryService.GetByIdAsync(key);
            return item == null ? NotFound() : Ok(item);
        }

        // POST /odata/Categories
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _facade.CategoryService.CreateAsync(dto);
                return Created(created); // OData Created => 201 + Location tới entity
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // PUT /odata/Categories(1)
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put([FromODataUri] int key, [FromBody] UpdateCategoryDTO dto)
        {
            if (key != dto.CategoryId) return BadRequest("Mismatched CategoryId");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _facade.CategoryService.UpdateAsync(dto);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // PATCH /odata/Categories(1)
        // Cho phép cập nhật một phần (Name/Description)
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Patch([FromODataUri] int key, [FromBody] Delta<UpdateCategoryDTO> delta)
        {
            if (delta == null) return BadRequest("Invalid payload.");

            var current = await _facade.CategoryService.GetByIdAsync(key);
            if (current == null) return NotFound();

            // chuyển DTO hiện tại sang UpdateDTO rồi apply patch
            var dto = new UpdateCategoryDTO
            {
                CategoryId = current.CategoryId,
                Name = current.Name,
                Description = current.Description
            };
            delta.Patch(dto);

            try
            {
                await _facade.CategoryService.UpdateAsync(dto);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }

        // DELETE /odata/Categories(1)
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            try
            {
                await _facade.CategoryService.DeleteAsync(key);
                return NoContent();
            }
            catch (CustomValidationException ex)
            {
                return BadRequest(new { message = ex.Message, errors = ex.Errors });
            }
        }
    }
}