using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.DTOs;
using OnlineShoeStoreLibrary.Models;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnlineShoeStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService service) : ControllerBase
    {
        private readonly AuthService _service = service; // объект сервиса для работы с авторизацией

        [AllowAnonymous] // Атрибут дающий доступ любому неавторизованному пользователю
        [HttpPost("login")] // Метод Post с конечной точкой login
        public async Task<ActionResult<string>> PostUser(LoginDto loginDto)
        {
            var token = await _service.AuthUserWithTokenAsync(loginDto); // атворизация с получением токена

            return token is null ?
                BadRequest() :
                Ok(token); // Если токен успешно сгенерирован возвращает его, в противном случае возвращает код 400 (Ошибка запроса)
        }
    }
}
