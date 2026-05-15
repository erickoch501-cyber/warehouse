using System.Windows;

namespace WarehouseApp.Views
{
    public partial class WarehouseDialog : Window
    {
        public string WarehouseName { get; private set; } = "";
        public string WarehouseAddress { get; private set; } = "";

        public WarehouseDialog()
        {
            InitializeComponent();
            WhNameTextBox.Focus();
        }

        public WarehouseDialog(string existingName, string existingAddress) : this()
        {
            WhNameTextBox.Text = existingName;
            WhAddressTextBox.Text = existingAddress;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(WhNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(WhAddressTextBox.Text))
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            WarehouseName = WhNameTextBox.Text.Trim();
            WarehouseAddress = WhAddressTextBox.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}