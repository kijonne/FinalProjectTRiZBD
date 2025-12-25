using System.Text.Json.Serialization;

namespace OnlineShoeStoreLibrary.DTOs
{
    /// <summary>
    /// Класс для передачи логина и пароля при входе
    /// </summary>
    /// <param name="login">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    public class LoginDto(string login, string password)
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = login;

        [JsonPropertyName("password")]
        public string Password { get; set; } = password;
    }
}
