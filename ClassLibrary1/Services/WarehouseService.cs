using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class WarehouseService
    {
        public ObservableCollection<Warehouse> Warehouses = new ObservableCollection<Warehouse>();

        public List<Warehouse> GetByOrganization(int orgId)
        {
            return Warehouses.Where(w => w.OrgId == orgId).ToList();
        }

        public void Add(Warehouse warehouse)
        {
            Warehouses.Add(warehouse);
        }

        public void Delete(Warehouse warehouse)
        {
            Warehouses.Remove(warehouse);
        }
    }
}