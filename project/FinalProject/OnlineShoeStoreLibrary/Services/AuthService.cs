using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.DTOs;
using OnlineShoeStoreLibrary.Models;
using OnlineShoeStoreLibrary.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShoeShopLibrary.Services
{
    /// <summary>
    /// Сервис авторизации пользователей
    /// </summary>
    /// <param name="context">Контекст БД</param>
    public class AuthService(OnlineShoeStoreContext context)
    {

        private readonly OnlineShoeStoreContext _context = context;
        private int _jwtActiveMinutes = 15;

        /// <summary>
        /// Проверка введенного пароля с хешем из бд
        /// </summary>
        /// <param name="password">Пароль, который ввел пользователь</param>
        /// <param name="passwordHash">Хешиованный пароль из БД</param>
        /// <returns>true, если пароль верный, иначе false</returns>
        private bool VerifyPassword(string password, string passwordHash)
            => BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);

        /// <summary>
        /// Аутентификации пользователя
        /// </summary>
        /// <param name="request">пароль и логин, который ввел пользователь</param>
        /// <returns>Объект пользователя при успехе, иначе null</returns>
        public async Task<User?> AuthUserAsync(LoginDto request)
        {
            string login = request.Login;
            string password = request.Password;

            // Проверка на пустые значения
            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))
                return null;

            // Поиск пользователя в БД
            var user = await GetUserByLoginAsync(login);
            if (user is null)
                return null;

            // Верификация пароля
            return VerifyPassword(password, user.PasswordHash) ? user : null; 
        }

        /// <summary>
        /// Авторизация пользователя с токеном
        /// </summary>
        /// <param name="request">Логин и Пароль, который ввел пользователь</param>
        /// <returns>jwt, если успех, иначе null</returns>
        public async Task<string?> AuthUserWithTokenAsync(LoginDto request)
        {
            string login = request.Login;
            string password = request.Password;

            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))             
                return null;

            var user = await GetUserByLoginAsync(login); 
            if (user is null)
                return null;

            return VerifyPassword(password, user.PasswordHash) ? await GenerateToken(user) : null;
        }

        /// <summary>
        /// Генерация jwt
        /// </summary>
        /// <param name="user">Объект пользователя</param>
        /// <returns>jwt токен</returns>
        private async Task<string> GenerateToken(User user)
        {
            int id = user.UserId;
            string login = user.Login;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var role = await GetUserRoleAsync(login);

            var claims = new Claim[]
            {
                new ("id", id.ToString()),
                new ("login", login),
                new ("role", role.Name),
            };

            var token = new JwtSecurityToken(
                signingCredentials: credentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtActiveMinutes),
                issuer: AuthOptions.issuer,
                audience: AuthOptions.audience);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Находит пользователя по логину
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>Объект пользователя</returns>
        private async Task<User> GetUserByLoginAsync(string login)
            => await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login) ?? null!;

        /// <summary>
        /// Получение роли пользователя по логину
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns>Роль пользователя</returns>
        public async Task<Role?> GetUserRoleAsync(string login)
        {
            var user = await _context.Users
                .Include(c => c.Role)
                .FirstOrDefaultAsync(cu => cu.Login == login);

            return user is not null ? user.Role : null;
        }
    }
}
