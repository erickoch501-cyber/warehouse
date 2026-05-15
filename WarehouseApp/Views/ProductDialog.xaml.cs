using System;
using System.Windows;
using Microsoft.Win32;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class ProductDialog : Window
    {
        public string ProductName { get; private set; } = "";
        public string CategoryName { get; private set; } = "";
        public string ManufacturerName { get; private set; } = "";
        public string SupplierName { get; private set; } = "";
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public int Discount { get; private set; }
        public string ImagePath { get; private set; } = "";

        public ProductDialog()
        {
            InitializeComponent();
        }

        public ProductDialog(string name, string category, string manufacturer, string supplier,
            decimal price, int stock, int discount, string imagePath) : this()
        {
            NameTextBox.Text = name;
            CategoryTextBox.Text = category;
            ManufacturerTextBox.Text = manufacturer;
            SupplierTextBox.Text = supplier;
            PriceTextBox.Text = price.ToString();
            StockTextBox.Text = stock.ToString();
            DiscountTextBox.Text = discount.ToString();
            ImagePathTextBox.Text = imagePath;

            if (!string.IsNullOrEmpty(imagePath))
                PreviewImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(imagePath));
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = dialog.FileName;
                PreviewImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ErrorTextBlock.Text = "Введите наименование";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(CategoryTextBox.Text))
            {
                ErrorTextBlock.Text = "Введите категорию";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(ManufacturerTextBox.Text))
            {
                ErrorTextBlock.Text = "Введите производителя";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(SupplierTextBox.Text))
            {
                ErrorTextBlock.Text = "Введите поставщика";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                ErrorTextBlock.Text = "Цена должна быть положительным числом";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (!int.TryParse(StockTextBox.Text, out int stock) || stock < 0)
            {
                ErrorTextBlock.Text = "Количество должно быть неотрицательным";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            if (!int.TryParse(DiscountTextBox.Text, out int discount) || discount < 0 || discount > 100)
            {
                ErrorTextBlock.Text = "Скидка от 0 до 100";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            ProductName = NameTextBox.Text.Trim();
            CategoryName = CategoryTextBox.Text.Trim();
            ManufacturerName = ManufacturerTextBox.Text.Trim();
            SupplierName = SupplierTextBox.Text.Trim();
            Price = price;
            Stock = stock;
            Discount = discount;
            ImagePath = ImagePathTextBox.Text;

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