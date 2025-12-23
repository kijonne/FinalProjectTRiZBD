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

namespace OnlineShoeStoreApp.Pages
{
    public class LoginModel(AuthService service) : PageModel
    {
        private AuthService _authService = service;

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnPostLoginAsync()
        {
            LoginDto login = new(User.Login, User.PasswordHash);

            var user = await _authService.AuthUserAsync(login);
            var role = await _authService.GetUserRoleAsync(User.Login);

            if (user is null)
                return Page();

            HttpContext.Session.SetString("FirstName", user.FirstName);
            HttpContext.Session.SetString("SecondName", user.LastName);
            HttpContext.Session.SetString("Patronymic", user.Patronymic);
            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("Role", role.Name);

            return RedirectToPage("/Shoes/Index");
        }

        public async Task<IActionResult> OnPostGuest()
        {
            HttpContext.Session.Clear();

            HttpContext.Session.SetString("Role", "guest");

            return RedirectToPage("/Shoes/Index");
        }
    }
}