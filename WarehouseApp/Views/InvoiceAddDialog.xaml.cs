using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseData.Models;

namespace WarehouseApp.Views
{
    public partial class InvoiceAddDialog : Window
    {
        public InvoiceType SelectedType { get; private set; }
        public Product SelectedProduct { get; private set; }
        public int Quantity { get; private set; }

        public InvoiceAddDialog(List<Product> products)
        {
            InitializeComponent();
            ProductCombo.ItemsSource = products;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ProductCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            SelectedType = ((ComboBoxItem)TypeCombo.SelectedItem).Tag.ToString() == "Incoming"
                ? InvoiceType.Incoming
                : InvoiceType.Outgoing;
            SelectedProduct = ProductCombo.SelectedItem as Product;
            Quantity = quantity;

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