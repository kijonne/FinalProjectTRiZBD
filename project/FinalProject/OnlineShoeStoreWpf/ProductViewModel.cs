using OnlineShoeStoreLibrary.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnlineShoeStoreWpf
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        private readonly Product _product;

        public Product Product => _product;

        public string PhotoPath => string.IsNullOrEmpty(_product.PhotoName)
            ? "images/picture.png"
            : $"images/shoes/{_product.PhotoName}";

        public decimal DiscountedPrice => Math.Round(_product.Price * (1 - _product.Discount / 100m));

        public bool IsOutOfStock => Product.Quantity == 0;

        public bool HasBigDiscount => Product.Discount > 15;

        public bool HasDiscount => _product.Discount > 0;

        public ProductViewModel(Product product)
        {
            _product = product;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}