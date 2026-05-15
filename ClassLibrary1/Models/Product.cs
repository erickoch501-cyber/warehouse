namespace WarehouseData.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Discount { get; set; }
        public string ImagePath { get; set; } = "";
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }

        private static int _counter = 0;

        public Product()
        {
            Id = ++_counter;
        }
    }
}