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

        public ObservableCollection<Product> Products { get; set; } = new();
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
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .ToListAsync();

            Products.Clear();
            foreach (var p in products)
            {
                p.PhotoPath = string.IsNullOrEmpty(p.PhotoName)
                    ? "images/picture.png"
                    : $"images/shoes/{p.PhotoName}";

                p.DiscountedPrice = Math.Round(p.Price * (1 - p.Discount / 100m));

                Products.Add(p);
            }

            // Опционально: приветствие для авторизованного
            if (_currentUser != null)
            {
                Title = $"Каталог обуви — Привет, {_currentUser.FirstName} {_currentUser.LastName}";
            }
        }
    }
}