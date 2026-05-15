using System.Windows;
using WarehouseApp.Views;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp
{
    public partial class MainWindow : Window
    {
        private OrganizationService _orgService;
        private WarehouseService _whService;
        private ProductService _productService;
        private InvoiceService _invoiceService;

        public MainWindow()
        {
            InitializeComponent();

            _orgService = new OrganizationService();
            _whService = new WarehouseService();
            _productService = new ProductService();
            _invoiceService = new InvoiceService();

            TestDataSeeder.Seed(_orgService, _whService, _productService);

            OrganizationsList.ItemsSource = _orgService.Organizations;
        }

        private void OpenWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrganizationsList.SelectedItem as Organization;
            if (selected == null)
            {
                MessageBox.Show("Выберите организацию");
                return;
            }

            var window = new WarehouseManagementWindow(selected, _orgService, _whService, _productService, _invoiceService);
            window.Owner = this;
            window.Show();
        }
    }
}