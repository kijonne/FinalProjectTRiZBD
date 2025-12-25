using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.DTOs;

namespace OnlineShoeStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService service) : ControllerBase
    {
        private readonly AuthService _service = service;

        [AllowAnonymous] // Позволяет доступ неавторизованным пользователям
        [HttpPost("login")] // POST запрос на /api/Auth/login
        public async Task<ActionResult<string>> PostUser(LoginDto loginDto)
        {
            var token = await _service.AuthUserWithTokenAsync(loginDto);

            return token is null ?
                BadRequest() :
                Ok(token);
        }
    }
}
