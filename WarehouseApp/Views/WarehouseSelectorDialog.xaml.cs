using System.Collections.ObjectModel;
using System.Windows;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class WarehouseSelectorDialog : Window
    {
        public Warehouse SelectedWarehouse { get; private set; }

        public WarehouseSelectorDialog(WarehouseService whService)
        {
            InitializeComponent();
            WarehousesList.ItemsSource = whService.Warehouses;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedWarehouse = WarehousesList.SelectedItem as Warehouse;
            if (SelectedWarehouse == null)
            {
                MessageBox.Show("Выберите склад");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}