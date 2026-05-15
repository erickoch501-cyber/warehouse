using System.Windows;

namespace WarehouseApp.Views
{
    public partial class OrganizationDialog : Window
    {
        public string OrganizationName { get; private set; } = "";

        public OrganizationDialog()
        {
            InitializeComponent();
            OrgNameTextBox.Focus();
        }

        public OrganizationDialog(string existingName) : this()
        {
            OrgNameTextBox.Text = existingName;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OrgNameTextBox.Text))
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            OrganizationName = OrgNameTextBox.Text.Trim();
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