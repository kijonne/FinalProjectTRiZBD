//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using OnlineShoeStoreLibrary.Contexts;
//using OnlineShoeStoreLibrary.Models;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace OnlineShoeStore.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly OnlineShoeStoreContext _context;
//        private readonly IConfiguration _config;

//        public AuthController(OnlineShoeStoreContext context, IConfiguration config)
//        {
//            _context = context;
//            _config = config;
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginModel model)
//        {
//            var user = await _context.Users
//                .Include(u => u.Role)
//                .FirstOrDefaultAsync(u => u.Login == model.Login && u.PasswordHash == model.Password);

//            if (user == null)
//                return Unauthorized("Неверный логин или пароль");

//            var claims = new[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.Login),
//                new Claim(ClaimTypes.Role, user.Role.Name)
//            };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _config["Jwt:Issuer"],
//                audience: _config["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddHours(2),
//                signingCredentials: creds);

//            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
//        }
//    }

//    public class LoginModel
//    {
//        public string Login { get; set; } = string.Empty;
//        public string Password { get; set; } = string.Empty;
//    }
//}