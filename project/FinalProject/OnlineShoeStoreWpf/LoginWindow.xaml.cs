using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
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

            var dbUser = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login);

            if (dbUser == null || !BCrypt.Net.BCrypt.EnhancedVerify(password, dbUser.PasswordHash))
            {
                ErrorTextBlock.Text = "Неверный логин или пароль";
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dbUser.Login),
                new Claim(ClaimTypes.NameIdentifier, dbUser.UserId.ToString()),
                new Claim("FullName", $"{dbUser.LastName} {dbUser.FirstName} {dbUser.Patronymic}".Trim()),
                new Claim(ClaimTypes.Role, dbUser.Role.Name)
            };

            App.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom"));
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}