using Microsoft.Win32;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OnlineShoeStoreWpf
{
    public partial class EditProductWindow : Window
    {
        private readonly OnlineShoeStoreContext _context = new();
        private readonly Product _product;
        public System.Collections.Generic.List<Category> Categories { get; set; }
        public System.Collections.Generic.List<Manufacturer> Manufacturers { get; set; }
        public System.Collections.Generic.List<Supplier> Suppliers { get; set; }

        public Category SelectedCategory { get; set; }
        public Manufacturer SelectedManufacturer { get; set; }
        public Supplier SelectedSupplier { get; set; }

        private Grid ProductGrid => (Grid)Content;

        /// <summary>
        /// Конструктор окна редактирования товара
        /// </summary>
        /// <param name="product">Товар для редактирования</param>
        public EditProductWindow(Product product)
        {
            InitializeComponent();

            _product = product ?? new Product();

            DataContext = this;
            Categories = _context.Categories.ToList();
            Manufacturers = _context.Manufacturers.ToList();
            Suppliers = _context.Suppliers.ToList();

            if (product != null)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
                SelectedManufacturer = Manufacturers.FirstOrDefault(m => m.ManufacturerId == product.ManufacturerId);
                SelectedSupplier = Suppliers.FirstOrDefault(s => s.SupplierId == product.SupplierId);
            }

            ProductGrid.DataContext = _product;
        }

        /// <summary>
        /// Обрабатывает выбор фотографии для товара
        /// Копирует выбранный файл в папку images/shoes
        /// </summary>
        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                string fileName = Path.GetFileName(dialog.FileName);
                string targetPath = Path.Combine(Directory.GetCurrentDirectory(), "images", "shoes", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(dialog.FileName, targetPath, true);

                _product.PhotoName = fileName;
            }
        }

        /// <summary>
        /// Сохраняет изменения товара в базе данных
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _product.CategoryId = SelectedCategory?.CategoryId ?? 0;
            _product.ManufacturerId = SelectedManufacturer?.ManufacturerId ?? 0;
            _product.SupplierId = SelectedSupplier?.SupplierId ?? 0;

            if (_product.ProductId == 0)
                _context.Products.Add(_product);
            else
                _context.Products.Update(_product);

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}