using System.Collections.ObjectModel;
using System.Windows;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class ProductManagementWindow : Window
    {
        public ProductManagementWindow(Warehouse warehouse, ProductService productService, InvoiceService invoiceService)
        {
            InitializeComponent();
            WarehouseTitle.Text = $"Товары на складе: {warehouse.WhName}";

            var products = productService.GetByWarehouse(warehouse.WhId);
            ProductsGrid.ItemsSource = new ObservableCollection<Product>(products);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}