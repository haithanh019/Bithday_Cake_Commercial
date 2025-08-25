using BusinessLogic.DTOs.Users;
using DataAccess.Services.FacadeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace BirthdayCakeAPI.Controllers
{
    public class AuthController : ODataController
    {
        private readonly IFacadeService _facade;

        public AuthController(IFacadeService facade)
        {
            _facade = facade;
        }

        // POST /odata/Auth/
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequestDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _facade.AuthService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(result);
        }
    }
}
