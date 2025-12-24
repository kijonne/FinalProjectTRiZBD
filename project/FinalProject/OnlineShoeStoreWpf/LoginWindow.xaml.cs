using Microsoft.EntityFrameworkCore;
using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.DTOs;
using System.Security.Claims;
using System.Windows;

namespace OnlineShoeStoreWpf
{
    public partial class LoginWindow : Window
    {
        private readonly OnlineShoeStoreContext _context = new();

        public ClaimsPrincipal CurrentUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Введите логин и пароль";
                return;
            }

            // Синхронный запрос вместо async
            var dbUser = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login);

            if (dbUser == null)
            {
                ErrorTextBlock.Text = "Неверный логин или пароль";
                return;
            }

            bool passwordValid = false;
            try
            {
                passwordValid = BCrypt.Net.BCrypt.EnhancedVerify(password, dbUser.PasswordHash);
            }
            catch
            {
                passwordValid = false;
            }

            if (!passwordValid)
            {
                ErrorTextBlock.Text = "Неверный логин или пароль";
                return;
            }

            // Создаём claims (как у тебя)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dbUser.Login),
                new Claim(ClaimTypes.NameIdentifier, dbUser.UserId.ToString()),
                new Claim("FullName", $"{dbUser.LastName} {dbUser.FirstName} {dbUser.Patronymic}".Trim()),
                new Claim(ClaimTypes.Role, dbUser.Role.Name)
            };

            App.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
            var mainWindow = new MainWindow();  // ← Открываем MainWindow
            mainWindow.Show();
            Close();  // ← Закрываем логин
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
            var mainWindow = new MainWindow();  // ← Открываем MainWindow
            mainWindow.Show();
            Close();  // ← Закрываем логин
        }
    }
}