using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class WarehouseManagementWindow : Window
    {
        private Organization _organization;
        private OrganizationService _orgService;
        private WarehouseService _whService;
        private ProductService _productService;
        private InvoiceService _invoiceService;
        private Warehouse _selectedWarehouse;
        private ObservableCollection<Product> _allProducts;

        public WarehouseManagementWindow(Organization organization, OrganizationService orgService, WarehouseService whService, ProductService productService, InvoiceService invoiceService)
        {
            InitializeComponent();
            _organization = organization;
            _orgService = orgService;
            _whService = whService;
            _productService = productService;
            _invoiceService = invoiceService;

            Title = $"Управление складом - {_organization.OrgName}";

            LoadWarehouses();
        }

        private void LoadWarehouses()
        {
            var warehouses = _whService.GetByOrganization(_organization.OrgId);
            WarehousesList.ItemsSource = new ObservableCollection<Warehouse>(warehouses);
            if (warehouses.Any())
            {
                WarehousesList.SelectedItem = warehouses.First();
            }
        }

        private void LoadProducts()
        {
            if (_selectedWarehouse != null)
            {
                _allProducts = new ObservableCollection<Product>(_productService.GetByWarehouse(_selectedWarehouse.WhId));
                ApplyFilter();

                var suppliers = _allProducts.Select(p => p.Supplier).Where(s => s != null).Distinct().ToList();
                SupplierFilter.ItemsSource = suppliers;
            }
        }

        private void ApplyFilter()
        {
            if (_allProducts == null) return;

            var query = _allProducts.AsEnumerable();

            var searchText = SearchBox.Text?.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchText));
            }

            var selectedSupplier = SupplierFilter.SelectedItem as Supplier;
            if (selectedSupplier != null)
            {
                query = query.Where(p => p.SupplierId == selectedSupplier.Id);
            }

            var selectedSort = (SortBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (selectedSort)
            {
                case "По цене (возраст)":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "По цене (убыв)":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "По остатку":
                    query = query.OrderBy(p => p.Stock);
                    break;
                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            ProductsGrid.ItemsSource = query.ToList();
        }

        private void WarehousesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedWarehouse = WarehousesList.SelectedItem as Warehouse;
            LoadProducts();
        }

        private void WarehousesList_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var contextMenu = WarehousesList.ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.IsOpen = true;
            }
        }

        private void AddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WarehouseDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                var newWh = new Warehouse(dialog.WarehouseName, dialog.WarehouseAddress, _organization.OrgId);
                _whService.Warehouses.Add(newWh);
                LoadWarehouses();
            }
        }

        private void EditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var selected = WarehousesList.SelectedItem as Warehouse;
            if (selected == null) { MessageBox.Show("Выберите склад"); return; }

            var dialog = new WarehouseDialog(selected.WhName, selected.WhAddress);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                selected.WhName = dialog.WarehouseName;
                selected.WhAddress = dialog.WarehouseAddress;
                LoadWarehouses();
            }
        }

        private void DeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var selected = WarehousesList.SelectedItem as Warehouse;
            if (selected == null) { MessageBox.Show("Выберите склад"); return; }

            if (MessageBox.Show($"Удалить склад \"{selected.WhName}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _whService.Warehouses.Remove(selected);
                LoadWarehouses();
            }
        }

        private void CreateInvoice(Product product, int quantity, InvoiceType type)
        {
            var invoice = new Invoice
            {
                WarehouseId = _selectedWarehouse.WhId,
                Warehouse = _selectedWarehouse,
                Type = type,
                Date = DateTime.Now,
                IsCompleted = true
            };

            invoice.Items.Add(new InvoiceItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = quantity,
                Price = product.Price
            });

            _invoiceService.Add(invoice);
        }

        private void AddInvoiceForm_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Выберите склад");
                return;
            }

            var dialog = new InvoiceFormDialog(_productService, _selectedWarehouse.WhId);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                foreach (var item in dialog.Items)
                {
                    var existingProduct = _productService.Products
                        .FirstOrDefault(p => p.WarehouseId == _selectedWarehouse.WhId && p.Name == item.Name);

                    if (existingProduct != null)
                    {
                        existingProduct.Stock += item.Quantity;

                        var invoice = new Invoice
                        {
                            WarehouseId = _selectedWarehouse.WhId,
                            Warehouse = _selectedWarehouse,
                            Type = dialog.SelectedType,
                            Date = DateTime.Now,
                            IsCompleted = true
                        };
                        invoice.Items.Add(new InvoiceItem
                        {
                            ProductId = existingProduct.Id,
                            Product = existingProduct,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                        _invoiceService.Add(invoice);
                    }
                    else
                    {
                        var newProduct = new Product
                        {
                            Name = item.Name,
                            Category = new Category { Id = 1, Name = item.Category },
                            CategoryId = 1,
                            Manufacturer = new Manufacturer { Id = 1, Name = item.Manufacturer },
                            ManufacturerId = 1,
                            Supplier = new Supplier { Id = 1, Name = item.Supplier },
                            SupplierId = 1,
                            Price = item.Price,
                            Stock = item.Quantity,
                            Discount = item.Discount,
                            ImagePath = item.ImagePath,
                            WarehouseId = _selectedWarehouse.WhId,
                            Warehouse = _selectedWarehouse
                        };
                        _productService.Products.Add(newProduct);

                        var invoice = new Invoice
                        {
                            WarehouseId = _selectedWarehouse.WhId,
                            Warehouse = _selectedWarehouse,
                            Type = dialog.SelectedType,
                            Date = DateTime.Now,
                            IsCompleted = true
                        };
                        invoice.Items.Add(new InvoiceItem
                        {
                            ProductId = newProduct.Id,
                            Product = newProduct,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                        _invoiceService.Add(invoice);
                    }
                }

                LoadProducts();
                MessageBox.Show($"Накладная создана. Обработано {dialog.Items.Count} товаров.");
            }
        }

        private void Incoming_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsGrid.SelectedItem as Product;
            if (selected == null) { MessageBox.Show("Выберите товар"); return; }

            var dialog = new InputDialog("Введите количество для поступления");
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && int.TryParse(dialog.Answer, out int quantity) && quantity > 0)
            {
                var oldStock = selected.Stock;
                selected.Stock += quantity;
                CreateInvoice(selected, quantity, InvoiceType.Incoming);
                LoadProducts();
                MessageBox.Show($"Поступление {quantity} шт. товара \"{selected.Name}\".\nБыло: {oldStock}, стало: {selected.Stock}\nНакладная создана.");
            }
        }

        private void Outgoing_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsGrid.SelectedItem as Product;
            if (selected == null) { MessageBox.Show("Выберите товар"); return; }

            var dialog = new InputDialog("Введите количество для списания");
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && int.TryParse(dialog.Answer, out int quantity) && quantity > 0)
            {
                if (selected.Stock < quantity)
                {
                    MessageBox.Show($"Недостаточно товара. Остаток: {selected.Stock}");
                    return;
                }
                var oldStock = selected.Stock;
                selected.Stock -= quantity;
                CreateInvoice(selected, quantity, InvoiceType.Outgoing);
                LoadProducts();
                MessageBox.Show($"Списание {quantity} шт. товара \"{selected.Name}\".\nБыло: {oldStock}, стало: {selected.Stock}\nНакладная создана.");
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsGrid.SelectedItem as Product;
            if (selected == null) { MessageBox.Show("Выберите товар"); return; }

            var oldStock = selected.Stock;

            var dialog = new ProductDialog(
                selected.Name,
                selected.Category?.Name ?? "",
                selected.Manufacturer?.Name ?? "",
                selected.Supplier?.Name ?? "",
                selected.Price,
                selected.Stock,
                selected.Discount,
                selected.ImagePath ?? "");
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                selected.Name = dialog.ProductName;
                if (selected.Category != null) selected.Category.Name = dialog.CategoryName;
                if (selected.Manufacturer != null) selected.Manufacturer.Name = dialog.ManufacturerName;
                if (selected.Supplier != null) selected.Supplier.Name = dialog.SupplierName;
                selected.Price = dialog.Price;

                int quantityDiff = dialog.Stock - oldStock;
                if (quantityDiff != 0)
                {
                    selected.Stock = dialog.Stock;
                    if (quantityDiff > 0)
                    {
                        CreateInvoice(selected, quantityDiff, InvoiceType.Incoming);
                        MessageBox.Show($"Поступление {quantityDiff} шт. Накладная создана.");
                    }
                    else
                    {
                        CreateInvoice(selected, -quantityDiff, InvoiceType.Outgoing);
                        MessageBox.Show($"Списание {-quantityDiff} шт. Накладная создана.");
                    }
                }

                selected.Discount = dialog.Discount;
                selected.ImagePath = dialog.ImagePath;
                LoadProducts();
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selected = ProductsGrid.SelectedItem as Product;
            if (selected == null) { MessageBox.Show("Выберите товар"); return; }

            if (MessageBox.Show($"Удалить товар \"{selected.Name}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _productService.Products.Remove(selected);
                LoadProducts();
            }
        }

        private void ProductsGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var hit = ProductsGrid.InputHitTest(e.GetPosition(ProductsGrid));
            var row = FindVisualParent<System.Windows.Controls.DataGridRow>(hit as DependencyObject);

            if (row != null)
            {
                ProductsGrid.SelectedItem = row.DataContext;
                ProductsGrid.ContextMenu.IsOpen = true;
            }
            else
            {
                var menu = new ContextMenu();
                var addInvoiceItem = new MenuItem { Header = "Добавить через накладную" };
                addInvoiceItem.Click += AddInvoiceForm_Click;
                menu.Items.Add(addInvoiceItem);
                menu.IsOpen = true;
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T found) return found;
                child = System.Windows.Media.VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private void Search_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void Filter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void Sort_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void MenuWarehouses_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OrganizationSelectorDialog(_orgService);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && dialog.SelectedOrganization != null)
            {
                var window = new WarehouseListWindow(dialog.SelectedOrganization, _whService, _productService, _invoiceService);
                window.Owner = this;
                window.ShowDialog();
            }
        }

        private void MenuProducts_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WarehouseSelectorDialog(_whService);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && dialog.SelectedWarehouse != null)
            {
                var window = new ProductManagementWindow(dialog.SelectedWarehouse, _productService, _invoiceService);
                window.Owner = this;
                window.ShowDialog();
            }
        }

        private void MenuInvoices_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WarehouseSelectorDialog(_whService);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true && dialog.SelectedWarehouse != null)
            {
                var window = new InvoicesListWindow(dialog.SelectedWarehouse, _invoiceService, _productService);
                window.Owner = this;
                window.ShowDialog();
            }
        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuReferences_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Система управления складом\n\n" +
                "Версия: 1.0\n" +
                "Разработчик: Студент\n\n" +
                "Функционал:\n" +
                "• Управление организациями и складами\n" +
                "• Учёт товаров (поступление/списание)\n" +
                "• Накладные с несколькими товарами\n" +
                "• Поиск, фильтр и сортировка товаров",
                "О программе",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}