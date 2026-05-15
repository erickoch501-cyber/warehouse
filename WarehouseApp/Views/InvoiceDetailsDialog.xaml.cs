using System.Windows;
using WarehouseData.Models;

namespace WarehouseApp.Views
{
    public partial class InvoiceDetailsDialog : Window
    {
        public InvoiceDetailsDialog(Invoice invoice)
        {
            InitializeComponent();

            NumberTextBlock.Text = invoice.Number;
            DateTextBlock.Text = invoice.Date.ToString("dd.MM.yyyy HH:mm");
            TypeTextBlock.Text = invoice.Type == InvoiceType.Incoming ? "Поступление" : "Списание";

            foreach (var item in invoice.Items)
            {
                item.Amount = item.Quantity * item.Price;
            }

            ItemsGrid.ItemsSource = invoice.Items;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}