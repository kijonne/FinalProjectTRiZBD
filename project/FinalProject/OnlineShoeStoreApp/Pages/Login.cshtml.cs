using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace OnlineShoeStoreApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly OnlineShoeStoreContext _context;
        private readonly PasswordHasher<object> _passwordHasher;

        public LoginModel(OnlineShoeStoreContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<object>();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Введите логин")]
            [Display(Name = "Логин")]
            public string Login { get; set; }

            [Required(ErrorMessage = "Введите пароль")]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == Input.Login);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return Page();
            }

            // Проверка пароля
            var result = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, Input.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return Page();
            }

            // Создание Claims для авторизации
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return LocalRedirect(ReturnUrl);
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

        // ВРЕМЕННЫЙ МЕТОД: Создание тестовых пользователей
        public async Task<IActionResult> OnPostCreateTestUsersAsync()
        {
            // Проверка - если уже есть admin, не создавать
            if (await _context.Users.AnyAsync(u => u.Login == "admin"))
            {
                ModelState.AddModelError(string.Empty, "Тестовые пользователи уже созданы");
                return Page();
            }
            var testUsers = new List<User>
            {
                new User
                {
                    RoleId = 1,
                    FirstName = "Админ",
                    LastName = "Админов",
                    Patronymic = "Админович",
                    Login = "admin",
                    PasswordHash = _passwordHasher.HashPassword(null, "admin")
                },
            };
            _context.Users.AddRange(testUsers);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Тестовые пользователи созданы!";
            return RedirectToPage();
        }

    }
}