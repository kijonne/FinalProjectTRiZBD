using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using OnlineShoeStoreWpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
namespace OnlineShoeStoreWpf
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly OnlineShoeStoreContext _context = new();
        public ObservableCollection<Product> Products { get; } = new();
        public ObservableCollection<Manufacturer> Manufacturers { get; } = new();
        public string SearchDescription { get; set; } = "";
        public Manufacturer SelectedManufacturer { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool OnlyDiscount { get; set; }
        public bool OnlyInStock { get; set; }
        public string SortOrder { get; set; } = "article_asc";
        public Product SelectedProduct { get; set; }
        public bool IsAdminOrManager => App.CurrentUser?.IsInRole("Администратор") == true || App.CurrentUser?.IsInRole("Менеджер") == true;
        public MainWindowViewModel()
        {
            LoadManufacturers();
            ApplyFilter();
        }
        private void LoadManufacturers()
        {
            Manufacturers.Clear();
            _context.Manufacturers.OrderBy(m => m.Name).ToList().ForEach(Manufacturers.Add);
        }
        public void ApplyFilter()
        {
            var query = _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.Supplier)
                .AsQueryable();

            // Фильтр по описанию
            if (!string.IsNullOrWhiteSpace(SearchDescription))
                query = query.Where(p => EF.Functions.Like(p.Description, $"%{SearchDescription}%"));

            // Фильтр по производителю
            if (SelectedManufacturer != null)
                query = query.Where(p => p.ManufacturerId == SelectedManufacturer.ManufacturerId);


            // Фильтр по максимальной цене
            if (MaxPrice.HasValue)
                query = query.Where(p => p.Price <= MaxPrice.Value);

            // Фильтр "только со скидкой"
            if (OnlyDiscount)
                query = query.Where(p => p.Discount > 0);

            // Фильтр "только в наличии"
            if (OnlyInStock)
                query = query.Where(p => p.Quantity > 0);

            // Сортировка в зависимости от выбранного порядка
            query = SortOrder switch
            {
                "supplier_asc" => query.OrderBy(p => p.Supplier.Name),
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Article)
            };
            Products.Clear();
            query.ToList().ForEach(Products.Add);
        }
        public void AddProduct()
        {
            var window = new EditProductWindow(null);
            if (window.ShowDialog() == true)
                ApplyFilter();
        }
        public void EditProduct(Product selected)
        {
            if (selected == null) return;
            var window = new EditProductWindow(selected);
            if (window.ShowDialog() == true)
                ApplyFilter();
        }
        public void DeleteProduct(Product selected)
        {
            if (selected == null) return;
            if (MessageBox.Show("Удалить товар?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Products.Remove(selected);
                _context.SaveChanges();
                ApplyFilter();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}