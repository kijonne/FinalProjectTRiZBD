using OnlineShoeStoreLibrary.Models;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace OnlineShoeStoreWpf
{
    public class ProductViewModel
    {
        private readonly Product _product;

        public ProductViewModel(Product product)
        {
            _product = product;
        }

        public int ProductId => _product.ProductId;
        public string Article => _product.Article;
        public int Price => _product.Price;
        public byte Discount => _product.Discount;
        public int Quantity => _product.Quantity;
        public string Description => _product.Description;
        public string CategoryName => _product.Category?.Name;
        public string ManufacturerName => _product.Manufacturer?.Name;
        public string SupplierName => _product.Supplier?.Name;
        public int ManufacturerId => _product.ManufacturerId;

        public bool HasDiscount => Discount > 0;

        // Итоговая цена со скидкой
        public decimal FinalPrice => Price * (100 - Discount) / 100m;

        // Путь к фото
        public string PhotoPath
        {
            get
            {
                if (string.IsNullOrEmpty(_product.PhotoName))
                    return "/images/picture.png";

                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "products", _product.PhotoName);
                return File.Exists(path) ? path : "/images/picture.png";
            }
        }
    }
}