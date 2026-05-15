using WarehouseData.Models;

namespace WarehouseData.Services
{
    public static class TestDataSeeder
    {
        public static void Seed(OrganizationService orgService, WarehouseService whService, ProductService productService)
        {
            orgService.Organizations.Clear();
            whService.Warehouses.Clear();
            productService.Products.Clear();

            var org1 = new Organization("ООО Coolсклад");
            var org2 = new Organization("ООО GoogleWare");

            orgService.Organizations.Add(org1);
            orgService.Organizations.Add(org2);

            var wh1 = new Warehouse("Основной склад", "Москва, ул. Примерная, д. 1", org1.OrgId);
            var wh2 = new Warehouse("Резервный склад", "Москва, ул. Запасная, д. 2", org1.OrgId);
            var wh3 = new Warehouse("Главный склад", "Санкт-Петербург, Невский пр., д. 3", org2.OrgId);

            whService.Warehouses.Add(wh1);
            whService.Warehouses.Add(wh2);
            whService.Warehouses.Add(wh3);

        }
    }
}