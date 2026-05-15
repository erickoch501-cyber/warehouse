using System.Collections.ObjectModel;

namespace WarehouseData.Models
{
    public class Organization
    {
        public static int OrgCounter = 0;
        public int OrgId { get; set; }
        public string OrgName { get; set; } = "";
        public ObservableCollection<Warehouse> Warehouses = new ObservableCollection<Warehouse>();

        public Organization() { }

        public Organization(string name)
        {
            OrgId = ++OrgCounter;
            OrgName = name;
        }
    }
}