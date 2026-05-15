using System;
using System.Collections.Generic;

namespace WarehouseData.Models
{
    public enum InvoiceType
    {
        Incoming,
        Outgoing
    }

    public class Invoice
    {
        private static int _counter = 0;

        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
        public InvoiceType Type { get; set; }
        public DateTime Date { get; set; }
        public string Number { get; set; } = "";
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public bool IsCompleted { get; set; }

        public Invoice()
        {
            Id = ++_counter;
            Date = DateTime.Now;
            Number = $"Н{DateTime.Now:yyyyMMddHHmmss}{Id}";
        }
    }

    public class InvoiceItem
    {
        private static int _counter = 0;

        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }

        public InvoiceItem()
        {
            Id = ++_counter;
        }
    }
}