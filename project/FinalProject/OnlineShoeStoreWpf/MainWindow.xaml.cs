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

        public ObservableCollection<ProductViewModel> Products { get; set; } = new();
        public MainWindow() : this(null) { }
        public MainWindow(User currentUser)
        {
            InitializeComponent();
            _context = new OnlineShoeStoreContext();
            _currentUser = currentUser ?? null;
            DataContext = this;

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadProducts();

            if (_currentUser != null && (_currentUser.Role.Name == "Администратор" || _currentUser.Role.Name == "Менеджер"))
            {
                var addButton = new Button { Content = "Добавить товар", Width = 150, Height = 40, Margin = new Thickness(10), Background = Brushes.LightGreen };
                addButton.Click += (s, e) => OpenEditForm(null);

                var editButton = new Button { Content = "Редактировать", Width = 150, Height = 40, Margin = new Thickness(10), Background = Brushes.MediumSpringGreen };
                editButton.Click += (s, e) => OpenEditForm(ProductsList.SelectedItem as ProductViewModel);

                var deleteButton = new Button { Content = "Удалить", Width = 150, Height = 40, Margin = new Thickness(10), Background = Brushes.IndianRed };
                deleteButton.Click += async (s, e) => await DeleteProduct(ProductsList.SelectedItem as ProductViewModel);

                AdminPanel.Children.Add(addButton);
                AdminPanel.Children.Add(editButton);
                AdminPanel.Children.Add(deleteButton);
            }
        }

        private void OpenEditForm(ProductViewModel vm)
        {
            var product = vm?.Product ?? new Product(); // Здесь берём Product из ViewModel
            var form = new EditProductWindow(product, _context);
            if (form.ShowDialog() == true)
            {
                LoadProducts(); // Обновляем список
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedVM = ProductsList.SelectedItem as ProductViewModel;
            if (selectedVM == null) return;

            var form = new EditProductWindow(selectedVM.Product, _context); // Здесь selectedVM.Product
            if (form.ShowDialog() == true)
            {
                LoadProducts();
            }
        }

        private void EditProduct(ProductViewModel vm)
        {
            if (vm == null) return;

            var form = new EditProductWindow(vm.Product, _context); // Передаём именно Product

            if (form.ShowDialog() == true)
            {
                LoadProducts(); // Обновляем список после сохранения
            }
        }

        private async Task DeleteProduct(ProductViewModel vm)
        {
            if (vm == null) return;

            _context.Products.Remove(vm.Product); // Здесь vm.Product
            await _context.SaveChangesAsync();
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            Products.Clear();

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .ToListAsync();

            foreach (var p in products)
            {
                Products.Add(new ProductViewModel(p));
            }
        }
    }
}