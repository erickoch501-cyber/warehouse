using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class InvoiceFormDialog : Window
    {
        public InvoiceType SelectedType { get; private set; }
        public ObservableCollection<InvoiceItemData> Items { get; private set; }

        private ProductService _productService;
        private int _warehouseId;
        private List<Product> _existingProducts;

        public InvoiceFormDialog(ProductService productService, int warehouseId)
        {
            InitializeComponent();
            _productService = productService;
            _warehouseId = warehouseId;
            Items = new ObservableCollection<InvoiceItemData>();
            ItemsGrid.ItemsSource = Items;
            TypeCombo.SelectedIndex = 0;

            // Загружаем существующие товары на складе
            _existingProducts = _productService.GetByWarehouse(warehouseId);
            ExistingProductCombo.ItemsSource = _existingProducts;
        }

        private void ExistingProductCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selected = ExistingProductCombo.SelectedItem as Product;
            if (selected != null)
            {
                NameBox.Text = selected.Name;
                CategoryBox.Text = selected.Category?.Name ?? "";
                ManufacturerBox.Text = selected.Manufacturer?.Name ?? "";
                SupplierBox.Text = selected.Supplier?.Name ?? "";
                PriceBox.Text = selected.Price.ToString();
                DiscountBox.Text = selected.Discount.ToString();
                ImagePathBox.Text = selected.ImagePath ?? "";
                QuantityBox.Text = "1";

                ExistingProductHint.Text = $"Товар \"{selected.Name}\" уже есть на складе. Остаток: {selected.Stock} шт.";
                ExistingProductHint.Visibility = Visibility.Visible;
            }
        }

        private void NameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var name = NameBox.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;

            var existingProduct = _existingProducts.FirstOrDefault(p => p.Name == name);

            if (existingProduct != null)
            {
                CategoryBox.Text = existingProduct.Category?.Name ?? "";
                ManufacturerBox.Text = existingProduct.Manufacturer?.Name ?? "";
                SupplierBox.Text = existingProduct.Supplier?.Name ?? "";
                PriceBox.Text = existingProduct.Price.ToString();
                DiscountBox.Text = existingProduct.Discount.ToString();
                ImagePathBox.Text = existingProduct.ImagePath ?? "";

                ExistingProductHint.Text = $"Товар \"{name}\" уже есть на складе. Остаток: {existingProduct.Stock} шт.";
                ExistingProductHint.Visibility = Visibility.Visible;
            }
            else
            {
                ExistingProductHint.Visibility = Visibility.Collapsed;
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                ImagePathBox.Text = dialog.FileName;
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                ErrorText.Text = "Введите наименование товара";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price <= 0)
            {
                ErrorText.Text = "Цена должна быть положительным числом";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                ErrorText.Text = "Количество должно быть положительным числом";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (!int.TryParse(DiscountBox.Text, out int discount) || discount < 0 || discount > 100)
            {
                ErrorText.Text = "Скидка от 0 до 100";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            ErrorText.Visibility = Visibility.Collapsed;

            var existingItem = Items.FirstOrDefault(i => i.Name == NameBox.Text.Trim());
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var existingProduct = _existingProducts.FirstOrDefault(p => p.Name == NameBox.Text.Trim());
                Items.Add(new InvoiceItemData
                {
                    Name = NameBox.Text.Trim(),
                    Category = CategoryBox.Text.Trim(),
                    Manufacturer = ManufacturerBox.Text.Trim(),
                    Supplier = SupplierBox.Text.Trim(),
                    Price = price,
                    Quantity = quantity,
                    Discount = discount,
                    ImagePath = ImagePathBox.Text,
                    IsNew = existingProduct == null
                });
            }

            NameBox.Text = "";
            CategoryBox.Text = "";
            ManufacturerBox.Text = "";
            SupplierBox.Text = "";
            PriceBox.Text = "";
            QuantityBox.Text = "";
            DiscountBox.Text = "0";
            ImagePathBox.Text = "";
            ExistingProductHint.Visibility = Visibility.Collapsed;
            ExistingProductCombo.SelectedItem = null;
            NameBox.Focus();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as InvoiceItemData;
            if (item != null)
                Items.Remove(item);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count == 0)
            {
                ErrorText.Text = "Добавьте хотя бы один товар в накладную";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            SelectedType = ((ComboBoxItem)TypeCombo.SelectedItem).Tag.ToString() == "Incoming"
                ? InvoiceType.Incoming
                : InvoiceType.Outgoing;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class InvoiceItemData
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public string Supplier { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public string ImagePath { get; set; }
        public bool IsNew { get; set; }
    }
}