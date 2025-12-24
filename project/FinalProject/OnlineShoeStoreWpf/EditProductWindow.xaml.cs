using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OnlineShoeStoreWpf
{
    public partial class EditProductWindow : Window
    {
        private readonly OnlineShoeStoreContext _context = new();
        private readonly Product _product;

        public EditProductWindow(Product product)
        {
            InitializeComponent();

            _product = product ?? new Product();

            // Загружаем справочники
            DataContext = this;
            Categories = _context.Categories.ToList();
            Manufacturers = _context.Manufacturers.ToList();
            Suppliers = _context.Suppliers.ToList();

            // Привязываем текущие значения
            if (product != null)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
                SelectedManufacturer = Manufacturers.FirstOrDefault(m => m.ManufacturerId == product.ManufacturerId);
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.SupplierId == product.SupplierId);
            }

            // Привязываем сам продукт
            ProductGrid.DataContext = _product;
        }

        public System.Collections.Generic.List<Category> Categories { get; set; }
        public System.Collections.Generic.List<Manufacturer> Manufacturers { get; set; }
        public System.Collections.Generic.List<Supplier> Suppliers { get; set; }

        public Category SelectedCategory { get; set; }
        public Manufacturer SelectedManufacturer { get; set; }
        public Supplier SelectedSupplier { get; set; }

        private Grid ProductGrid => (Grid)Content; // Чтобы DataContext работал

        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                string fileName = Path.GetFileName(dialog.FileName);
                string targetPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "shoes", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)); // Создаёт папку, если нет
                File.Copy(dialog.FileName, targetPath, true);

                _product.PhotoName = fileName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняем связи
            _product.CategoryId = SelectedCategory?.CategoryId ?? 0;
            _product.ManufacturerId = SelectedManufacturer?.ManufacturerId ?? 0;
            _product.SupplierId = SelectedSupplier?.SupplierId ?? 0;

            if (_product.ProductId == 0)
            {
                _context.Products.Add(_product);
            }
            else
            {
                _context.Products.Update(_product);
            }

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}