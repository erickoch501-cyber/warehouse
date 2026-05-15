using System.Collections.ObjectModel;
using System.Linq;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class InvoiceService
    {
        public ObservableCollection<Invoice> Invoices { get; set; } = new ObservableCollection<Invoice>();

        public ObservableCollection<Invoice> GetByWarehouse(int warehouseId)
        {
            var result = new ObservableCollection<Invoice>();
            foreach (var invoice in Invoices)
            {
                if (invoice.WarehouseId == warehouseId)
                    result.Add(invoice);
            }
            return result;
        }

        public void Add(Invoice invoice)
        {
            Invoices.Add(invoice);
        }

        public void ProcessInvoice(Invoice invoice, ProductService productService)
        {
            if (invoice.IsCompleted) return;

            foreach (var item in invoice.Items)
            {
                var product = productService.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    if (invoice.Type == InvoiceType.Incoming)
                    {
                        product.Stock += item.Quantity;
                    }
                    else if (invoice.Type == InvoiceType.Outgoing)
                    {
                        product.Stock -= item.Quantity;
                        if (product.Stock < 0) product.Stock = 0;
                    }
                }
            }

            invoice.IsCompleted = true;
        }
    }
}