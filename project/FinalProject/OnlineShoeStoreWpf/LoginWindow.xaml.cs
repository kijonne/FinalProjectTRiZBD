using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OnlineShoeStoreWpf
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            var context = new OnlineShoeStoreContext();
            _authService = new AuthService(context);
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            var main = new MainWindow(null);
            main.Show();
            this.Close();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text.Trim();
            var password = PasswordBox.Password;

            MessageBox.Show($"Пытаюсь войти: логин = '{login}', пароль = '{password}'");

            var dto = new LoginDto(login, password);
            var user = await _authService.AuthUserAsync(dto);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден или пароль неверный");
                return;
            }

            MessageBox.Show($"Успех! Роль: {user.Role.Name}, ФИО: {user.FirstName} {user.LastName}");

            var main = new MainWindow(user);
            main.Show();
            this.Close();
        }
    }
}
