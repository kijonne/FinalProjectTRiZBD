using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.DTOs;
using System.Windows;

namespace OnlineShoeStoreWpf
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly OnlineShoeStoreContext _context;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new OnlineShoeStoreContext();
            _authService = new AuthService(_context);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Заполните все поля");
                return;
            }

            try
            {
                var dto = new LoginDto(login, password);
                var user = await _authService.AuthUserAsync(dto);

                if (user == null)
                {
                    ShowError("Неверный логин или пароль");
                    return;
                }

                // Успешный вход
                var mainWindow = new MainWindow(_context, user);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Гостевой вход как клиент
                var dto = new LoginDto("client", "client");
                var user = await _authService.AuthUserAsync(dto);

                if (user == null)
                {
                    ShowError("Гостевой вход недоступен");
                    return;
                }

                var mainWindow = new MainWindow(_context, user);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}