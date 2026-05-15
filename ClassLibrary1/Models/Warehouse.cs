using System.Collections.Generic;

namespace WarehouseData.Models
{
    public class Warehouse
    {
        public static int WarehousesCounter = 0;
        public int WhId { get; set; }
        public string WhName { get; set; } = "";
        public string WhAddress { get; set; } = "";
        public int OrgId { get; set; }
        public Organization Organization { get; set; }
        public List<Product> Products = new List<Product>();

        public Warehouse() { }

        public Warehouse(string name, string address, int orgid)
        {
            WhId = ++WarehousesCounter;
            WhName = name;
            WhAddress = address;
            OrgId = orgid;
        }
    }
}