using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarehouseData.Models;
using WarehouseData.Services;

namespace WarehouseApp.Views
{
    public partial class InvoicesListWindow : Window
    {
        private Warehouse _warehouse;
        private InvoiceService _invoiceService;
        private ProductService _productService;
        private ObservableCollection<Invoice> _invoices;

        public InvoicesListWindow(Warehouse warehouse, InvoiceService invoiceService, ProductService productService)
        {
            InitializeComponent();
            _warehouse = warehouse;
            _invoiceService = invoiceService;
            _productService = productService;

            WarehouseTitle.Text = $"Накладные склада: {_warehouse.WhName}";
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            _invoices = new ObservableCollection<Invoice>(_invoiceService.GetByWarehouse(_warehouse.WhId));
            InvoicesGrid.ItemsSource = _invoices;
        }

        private void AddInvoice_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InvoiceFormDialog(_productService, _warehouse.WhId);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                foreach (var item in dialog.Items)
                {
                    var existingProduct = _productService.Products
                        .FirstOrDefault(p => p.WarehouseId == _warehouse.WhId && p.Name == item.Name);

                    if (existingProduct != null)
                    {
                        existingProduct.Stock += item.Quantity;

                        var invoice = new Invoice
                        {
                            WarehouseId = _warehouse.WhId,
                            Warehouse = _warehouse,
                            Type = dialog.SelectedType,
                            Date = System.DateTime.Now,
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
                            WarehouseId = _warehouse.WhId,
                            Warehouse = _warehouse
                        };
                        _productService.Products.Add(newProduct);

                        var invoice = new Invoice
                        {
                            WarehouseId = _warehouse.WhId,
                            Warehouse = _warehouse,
                            Type = dialog.SelectedType,
                            Date = System.DateTime.Now,
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

                LoadInvoices();
                MessageBox.Show($"Накладная создана. Обработано {dialog.Items.Count} товаров.");
            }
        }

        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            var selected = InvoicesGrid.SelectedItem as Invoice;
            if (selected == null)
            {
                MessageBox.Show("Выберите накладную");
                return;
            }

            var dialog = new InvoiceDetailsDialog(selected);
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void InvoicesGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ViewInvoice_Click(sender, e);
        }

        private void InvoicesGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var hit = InvoicesGrid.InputHitTest(e.GetPosition(InvoicesGrid));
            var row = FindVisualParent<System.Windows.Controls.DataGridRow>(hit as DependencyObject);

            if (row != null)
            {
                InvoicesGrid.SelectedItem = row.DataContext;
                InvoicesGrid.ContextMenu.IsOpen = true;
            }
            else
            {
                var menu = new ContextMenu();
                var addItem = new MenuItem { Header = "Добавить накладную" };
                addItem.Click += AddInvoice_Click;
                menu.Items.Add(addItem);
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}