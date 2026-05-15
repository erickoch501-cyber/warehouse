using System.Collections.ObjectModel;
using System.Windows;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class OrganizationSelectorDialog : Window
    {
        public Organization SelectedOrganization { get; private set; }

        public OrganizationSelectorDialog(OrganizationService orgService)
        {
            InitializeComponent();
            OrganizationsList.ItemsSource = orgService.Organizations;

            if (orgService.Organizations.Count > 0)
            {
                OrganizationsList.SelectedItem = orgService.Organizations[0];
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedOrganization = OrganizationsList.SelectedItem as Organization;
            if (SelectedOrganization == null)
            {
                MessageBox.Show("Выберите организацию", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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