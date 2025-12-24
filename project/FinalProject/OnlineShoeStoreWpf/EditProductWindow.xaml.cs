using Microsoft.Win32;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для EditProductWindow.xaml
    /// </summary>
    public partial class EditProductWindow : Window
    {
        private readonly OnlineShoeStoreContext _context;
        private Product _product;

        public EditProductWindow(Product product, OnlineShoeStoreContext context)
        {
            InitializeComponent();
            _context = context;
            _product = product ?? new Product();

            // Загрузка ComboBox
            CategoryComboBox.ItemsSource = _context.Categories.ToList();
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "CategoryId";

            ManufacturerComboBox.ItemsSource = _context.Manufacturers.ToList();
            ManufacturerComboBox.DisplayMemberPath = "Name";
            ManufacturerComboBox.SelectedValuePath = "ManufacturerId";

            SupplierComboBox.ItemsSource = _context.Suppliers.ToList();
            SupplierComboBox.DisplayMemberPath = "Name";
            SupplierComboBox.SelectedValuePath = "SupplierId";

            // Если редактирование — заполняем поля
            if (product != null)
            {
                Title = "Редактирование товара";
                ArticleTextBox.Text = product.Article;
                PriceTextBox.Text = product.Price.ToString();
                DiscountTextBox.Text = product.Discount.ToString();
                QuantityTextBox.Text = product.Quantity.ToString();
                DescriptionTextBox.Text = product.Description ?? "";
                SizeTextBox.Text = product.Size.ToString();
                ColorTextBox.Text = product.Color ?? "";
                CategoryComboBox.SelectedValue = product.CategoryId;
                ManufacturerComboBox.SelectedValue = product.ManufacturerId;
                SupplierComboBox.SelectedValue = product.SupplierId;
            }
            else
            {
                Title = "Добавление нового товара";
            }
        }

        private void ChoosePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                string destFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "shoes");
                Directory.CreateDirectory(destFolder); // Создаёт папку если нет

                string destPath = System.IO.Path.Combine(destFolder, fileName);
                File.Copy(openFileDialog.FileName, destPath, true);

                _product.PhotoName = fileName; // Сохраняем только имя файла в БД
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _product.Article = ArticleTextBox.Text;
            _product.Price = int.Parse(PriceTextBox.Text);
            _product.Discount = byte.Parse(DiscountTextBox.Text);
            _product.Quantity = int.Parse(QuantityTextBox.Text);
            _product.Description = DescriptionTextBox.Text;
            _product.Size = byte.Parse(SizeTextBox.Text);
            _product.Color = ColorTextBox.Text;
            _product.CategoryId = (int)CategoryComboBox.SelectedValue;
            _product.ManufacturerId = (int)ManufacturerComboBox.SelectedValue;
            _product.SupplierId = (int)SupplierComboBox.SelectedValue;

            if (_product.ProductId == 0)
                _context.Products.Add(_product);
            else
                _context.Products.Update(_product);

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
