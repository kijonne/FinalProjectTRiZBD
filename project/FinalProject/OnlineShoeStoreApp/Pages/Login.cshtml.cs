using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.DTOs;
using OnlineShoeStoreLibrary.Models;
using OnlineShoeShopLibrary.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BCrypt.Net;

namespace OnlineShoeStoreApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly OnlineShoeStoreContext _context;

        public LoginModel(OnlineShoeStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = new User(); // инициализируем, чтобы не null

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(User.Login) || string.IsNullOrWhiteSpace(User.PasswordHash))
            {
                ErrorMessage = "Введите логин и пароль";
                return Page();
            }

            // Находим пользователя по логину
            var dbUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == User.Login);

            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(User.PasswordHash, dbUser.PasswordHash))
            {
                ErrorMessage = "Неверный логин или пароль";
                return Page();
            }

            // Создаём claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dbUser.Login),
                new Claim(ClaimTypes.NameIdentifier, dbUser.UserId.ToString()),
                new Claim(ClaimTypes.GivenName, dbUser.FirstName),
                new Claim(ClaimTypes.Surname, dbUser.LastName),
                new Claim(ClaimTypes.Role, dbUser.Role.Name)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Products/Index");
        }

        public IActionResult OnPostGuest()
        {
            return RedirectToPage("/Products/Index");
        }

        public async Task<IActionResult> OnGetLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Index");
        }
    }
}
