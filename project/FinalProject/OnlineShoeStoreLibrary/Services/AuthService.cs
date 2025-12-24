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
    /// Сервис для работы с авторизацией пользователей
    /// </summary>
    /// <param name="context">контекст базы данных</param>
    public class AuthService(OnlineShoeStoreContext context)
    {

        private readonly OnlineShoeStoreContext _context = context;
        private int _jwtActiveMinutes = 15; //срок действия jwt ключа в минутах

        /// <summary>
        /// Проверка корректности пароля (хеша)
        /// </summary>
        /// <param name="password">пароль введённый пользователем</param>
        /// <param name="passwordHash">хешиованный пароль из БД</param>
        /// <returns>Если пароль верный возвращает true, в противном случае false</returns>
        private bool VerifyPassword(string password, string passwordHash)
            => BCrypt.Net.BCrypt.Verify(password, passwordHash);

        /// <summary>
        /// Метод аутентификации пользователя
        /// </summary>
        /// <param name="request">пароль и логин введёный пользователём</param>
        /// <returns>Если авторизация прошла успешна, возвращает объект пользователя</returns>
        public async Task<User?> AuthUserAsync(LoginDto request)
        {
            string login = request.Login;
            string password = request.Password;

            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password)) //Проверка на пустую строку
                return null;

            var user = await GetUserByLoginAsync(login); //Получение пользователя по логину (если такой есть)
            if (user is null)
                return null;

            return VerifyPassword(password, user.PasswordHash) ? user : null; 
            //Авторизация пользователя, если всё прошло корректно
            //возвращается объект пользователя, в противном случае null
        }

        /// <summary>
        /// Авторизация пользователя с токеном
        /// </summary>
        /// <param name="request">Логин и Пароль введённый пользователем</param>
        /// <returns>jwt</returns>
        public async Task<string?> AuthUserWithTokenAsync(LoginDto request)
        {
            string login = request.Login;
            string password = request.Password;

            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password)) //Проверка на пустую строку
                return null;

            var user = await GetUserByLoginAsync(login); //Получение пользователя по логину (если такой есть)
            if (user is null)
                return null;

            return VerifyPassword(password, user.PasswordHash) ? await GenerateToken(user) : null;
            //Авторизация пользователя, если всё прошло корректно
            //возвращается jwt, в противном случае null
        }

        /// <summary>
        /// Генерация jwt
        /// </summary>
        /// <param name="user">Объект пользователя</param>
        /// <returns>Сгенерированный jwt</returns>
        private async Task<string> GenerateToken(User user)
        {
            int id = user.UserId;
            string login = user.Login;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var role = await GetUserRoleAsync(login); // Получение роли пользователя

            var claims = new Claim[]
            {
                new ("id", id.ToString()),
                new ("login", login),
                new ("role", role.Name),
            }; // берём данные для записи в jwt

            var token = new JwtSecurityToken(
                signingCredentials: credentials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtActiveMinutes), //указываем срок, до которого активен ключ
                issuer: AuthOptions.issuer,
                audience: AuthOptions.audience); // заполняем данные приложения для генерации токена

            return new JwtSecurityTokenHandler().WriteToken(token); //Записываем токен
        }

        /// <summary>
        /// Получение пользователя по логину
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
                .Include(c => c.Role) //дополнительно загружает Роль
                .FirstOrDefaultAsync(cu => cu.Login == login); // Ассинхронный метод выборки первого объекта, где совпал логин

            return user is not null ?
                 user.Role :
                 null; // если пользователь существует, возвращает роль
        }
    }
}
