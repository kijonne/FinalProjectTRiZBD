using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineShoeStoreLibrary.DTOs
{
    public class LoginDto(string login, string password)
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = login;

        [JsonPropertyName("password")]
        public string Password { get; set; } = password;
    }
}
