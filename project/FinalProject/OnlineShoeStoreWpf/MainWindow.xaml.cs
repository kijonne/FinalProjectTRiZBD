using System.Windows;
namespace OnlineShoeStoreWpf
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent(); 
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApplyFilter();
        }
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddProduct();
        }
        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            //ViewModel.EditProduct();
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            //ViewModel.DeleteProduct();
        }
    }
}