using System.Collections.ObjectModel;
using System.Windows;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class WarehouseListWindow : Window
    {
        public WarehouseListWindow(Organization organization, WarehouseService whService, ProductService productService, InvoiceService invoiceService)
        {
            InitializeComponent();
            OrgTitle.Text = $"Склады организации: {organization.OrgName}";

            var warehouses = whService.GetByOrganization(organization.OrgId);
            WarehousesGrid.ItemsSource = new ObservableCollection<Warehouse>(warehouses);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}