using Microsoft.EntityFrameworkCore;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Models;
using OnlineShoeStoreWpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            ViewModel.EditProduct(DataGrid.SelectedItem as Product);
        }
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteProduct(DataGrid.SelectedItem as Product);
        }
    }
}