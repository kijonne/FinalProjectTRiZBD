using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using System.Security.Claims;

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
        public InputModel Input { get; set; } = new(); 

        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [BindProperty]
            public string Login { get; set; }
            [BindProperty]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Login) || string.IsNullOrWhiteSpace(Input.Password))
            {
                ErrorMessage = "Введите логин и пароль";
                return Page();
            }

            var dbUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == Input.Login);

            if (dbUser == null || !BCrypt.Net.BCrypt.EnhancedVerify(Input.Password, dbUser.PasswordHash))
            {
                ErrorMessage = "Неверный логин или пароль";
                return Page();
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dbUser.Login),
                    new Claim(ClaimTypes.NameIdentifier, dbUser.UserId.ToString()),
                    new Claim("FullName", $"{dbUser.LastName} {dbUser.FirstName} {dbUser.Patronymic}".Trim()),
                    new Claim(ClaimTypes.Role, dbUser.Role.Name)
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect("~/Products/Index");
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
