using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class ProductService
    {
        public ObservableCollection<Product> Products = new ObservableCollection<Product>();

        public List<Product> GetByWarehouse(int warehouseId)
        {
            return Products.Where(p => p.WarehouseId == warehouseId).ToList();
        }

        public void Add(Product product)
        {
            Products.Add(product);
        }

        public void Delete(Product product)
        {
            Products.Remove(product);
        }
    }
}