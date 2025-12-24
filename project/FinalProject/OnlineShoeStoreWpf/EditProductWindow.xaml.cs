using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OnlineShoeStoreWpf
{
    public partial class EditProductWindow : Window
    {
        private readonly OnlineShoeStoreContext _context;
        private readonly int? _productId;
        private Product _product;
        private string _selectedPhotoPath;

        public EditProductWindow(OnlineShoeStoreContext context, int? productId = null)
        {
            InitializeComponent();
            _context = context;
            _productId = productId;

            LoadData();
        }

        private async void LoadData()
        {
            // Загрузка справочников
            CategoryCombo.ItemsSource = await _context.Categories.ToListAsync();
            ManufacturerCombo.ItemsSource = await _context.Manufacturers.ToListAsync();
            SupplierCombo.ItemsSource = await _context.Suppliers.ToListAsync();

            if (_productId.HasValue)
            {
                // Режим редактирования
                _product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Supplier)
                    .FirstOrDefaultAsync(p => p.ProductId == _productId.Value);

                if (_product != null)
                {
                    ArticleBox.Text = _product.Article;
                    PriceBox.Text = _product.Price.ToString();
                    DiscountBox.Text = _product.Discount.ToString();
                    QuantityBox.Text = _product.Quantity.ToString();
                    DescriptionBox.Text = _product.Description;
                    SizeBox.Text = _product.Size?.ToString();
                    ColorBox.Text = _product.Color;
                    GenderCombo.SelectedIndex = _product.Gender ? 1 : 0;
                    CategoryCombo.SelectedValue = _product.CategoryId;
                    ManufacturerCombo.SelectedValue = _product.ManufacturerId;
                    SupplierCombo.SelectedValue = _product.SupplierId;

                    // Отображение фото
                    if (!string.IsNullOrEmpty(_product.PhotoName))
                    {
                        var photoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "products", _product.PhotoName);
                        if (File.Exists(photoPath))
                        {
                            ProductImage.Source = new BitmapImage(new Uri(photoPath));
                            PhotoNameText.Text = _product.PhotoName;
                        }
                    }
                }
            }
            else
            {
                // Режим создания
                _product = new Product();
                GenderCombo.SelectedIndex = 0;
            }
        }

        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите фото товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPhotoPath = openFileDialog.FileName;
                ProductImage.Source = new BitmapImage(new Uri(_selectedPhotoPath));
                PhotoNameText.Text = Path.GetFileName(_selectedPhotoPath);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(ArticleBox.Text))
            {
                MessageBox.Show("Введите артикул", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(PriceBox.Text, out int price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!byte.TryParse(DiscountBox.Text, out byte discount) || discount > 100)
            {
                MessageBox.Show("Скидка должна быть от 0 до 100", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CategoryCombo.SelectedValue == null || ManufacturerCombo.SelectedValue == null || SupplierCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию, производителя и поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохранение фото
            if (!string.IsNullOrEmpty(_selectedPhotoPath))
            {
                var fileName = Path.GetFileName(_selectedPhotoPath);
                var destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "products", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                File.Copy(_selectedPhotoPath, destPath, true);

                _product.PhotoName = fileName;
            }

            // Заполнение модели
            _product.Article = ArticleBox.Text.Trim();
            _product.Price = price;
            _product.Discount = discount;
            _product.Quantity = quantity;
            _product.Description = DescriptionBox.Text?.Trim();
            _product.Size = byte.TryParse(SizeBox.Text, out byte size) ? size : null;
            _product.Color = ColorBox.Text?.Trim();
            _product.Gender = GenderCombo.SelectedIndex == 1;
            _product.CategoryId = (int)CategoryCombo.SelectedValue;
            _product.ManufacturerId = (int)ManufacturerCombo.SelectedValue;
            _product.SupplierId = (int)SupplierCombo.SelectedValue;
            _product.Unit = "шт.";

            // Сохранение в БД
            if (_productId.HasValue)
            {
                _context.Products.Update(_product);
            }
            else
            {
                _context.Products.Add(_product);
            }

            await _context.SaveChangesAsync();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}