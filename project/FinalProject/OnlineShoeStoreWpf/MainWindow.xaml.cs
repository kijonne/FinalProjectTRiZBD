using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineShoeStoreWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OnlineShoeStoreContext _context;
        private readonly User _currentUser;

        private ObservableCollection<ProductViewModel> _allProducts;
        private ObservableCollection<ProductViewModel> _filteredProducts;

        public MainWindow(OnlineShoeStoreContext context, User user)
        {
            InitializeComponent();
            _context = context;
            _currentUser = user;

            LoadData();
            SetupUI();
        }

        private async void LoadData()
        {
            // Загрузка товаров
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .ToListAsync();

            _allProducts = new ObservableCollection<ProductViewModel>(
                products.Select(p => new ProductViewModel(p))
            );

            _filteredProducts = new ObservableCollection<ProductViewModel>(_allProducts);
            ProductsList.ItemsSource = _filteredProducts;

            // Загрузка производителей для фильтра
            var manufacturers = await _context.Manufacturers.ToListAsync();
            ManufacturerFilter.Items.Add(new ComboBoxItem { Content = "Все", Tag = 0 });
            foreach (var m in manufacturers)
            {
                ManufacturerFilter.Items.Add(new ComboBoxItem { Content = m.Name, Tag = m.ManufacturerId });
            }
            ManufacturerFilter.SelectedIndex = 0;
        }

        private void SetupUI()
        {
            // Отображение ФИО пользователя
            UserNameText.Text = $"{_currentUser.FirstName} {_currentUser.LastName}";
            LogoutButton.Visibility = Visibility.Visible;

            // Показать кнопки для админа/менеджера
            if (_currentUser.Role.Name == "Admin" || _currentUser.Role.Name == "Manager")
            {
                AdminPanel.Visibility = Visibility.Visible;
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allProducts.AsEnumerable();

            // Поиск
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                filtered = filtered.Where(p => p.Description?.Contains(SearchBox.Text, StringComparison.OrdinalIgnoreCase) == true);
            }

            // Фильтр по производителю
            if (ManufacturerFilter.SelectedItem is ComboBoxItem selectedManuf && (int)selectedManuf.Tag != 0)
            {
                filtered = filtered.Where(p => p.ManufacturerId == (int)selectedManuf.Tag);
            }

            // Фильтр по цене
            if (decimal.TryParse(MaxPriceBox.Text, out var maxPrice))
            {
                filtered = filtered.Where(p => p.Price <= maxPrice);
            }

            // Только со скидкой
            if (OnlyDiscountCheck.IsChecked == true)
            {
                filtered = filtered.Where(p => p.Discount > 0);
            }

            // Только в наличии
            if (OnlyInStockCheck.IsChecked == true)
            {
                filtered = filtered.Where(p => p.Quantity > 0);
            }

            // Сортировка
            filtered = SortComboBox.SelectedIndex switch
            {
                1 => filtered.OrderBy(p => p.FinalPrice),
                2 => filtered.OrderByDescending(p => p.FinalPrice),
                3 => filtered.OrderBy(p => p.SupplierName),
                _ => filtered.OrderBy(p => p.CategoryName).ThenBy(p => p.ManufacturerName)
            };

            _filteredProducts.Clear();
            foreach (var item in filtered)
            {
                _filteredProducts.Add(item);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void ManufacturerFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void MaxPriceBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void FilterCheckBox_Changed(object sender, RoutedEventArgs e) => ApplyFilters();
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new EditProductWindow(_context);
            if (addWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is ProductViewModel selected)
            {
                var editWindow = new EditProductWindow(_context, selected.ProductId);
                if (editWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is ProductViewModel selected)
            {
                var result = MessageBox.Show($"Удалить товар {selected.Article}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var product = await _context.Products.FindAsync(selected.ProductId);
                    if (product != null)
                    {
                        _context.Products.Remove(product);
                        await _context.SaveChangesAsync();
                        LoadData();
                    } 
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
