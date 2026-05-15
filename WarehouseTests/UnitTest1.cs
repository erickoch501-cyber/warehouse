using Xunit;
using WarehouseData.Models;
using WarehouseData.Services;
using System.Linq;

namespace WarehouseTests
{
    public class OrganizationServiceTests
    {
        private OrganizationService CreateCleanService()
        {
            var service = new OrganizationService();
            service.Organizations.Clear();
            return service;
        }

        [Fact]
        public void Add_ShouldAddOrganization()
        {
            var service = CreateCleanService();
            var org = new Organization("ООО Coolсклад");

            service.Organizations.Add(org);

            Assert.Single(service.Organizations);
            Assert.Equal("ООО Coolсклад", service.Organizations[0].OrgName);
        }

        [Fact]
        public void Delete_ShouldRemoveOrganization()
        {
            var service = CreateCleanService();
            var org = new Organization("ООО Coolсклад");
            service.Organizations.Add(org);

            service.Organizations.Remove(org);

            Assert.Empty(service.Organizations);
        }

        [Fact]
        public void Update_ShouldChangeOrganizationName()
        {
            var service = CreateCleanService();
            var org = new Organization("Старое название");
            service.Organizations.Add(org);

            org.OrgName = "Новое название";

            Assert.Equal("Новое название", service.Organizations[0].OrgName);
        }
    }

    public class WarehouseServiceTests
    {
        private WarehouseService CreateCleanService()
        {
            var service = new WarehouseService();
            service.Warehouses.Clear();
            return service;
        }

        [Fact]
        public void Add_ShouldAddWarehouse()
        {
            var service = CreateCleanService();
            var org = new Organization("Тест");
            var warehouse = new Warehouse("Склад 1", "Адрес 1", org.OrgId);

            service.Warehouses.Add(warehouse);

            Assert.Single(service.Warehouses);
            Assert.Equal("Склад 1", service.Warehouses[0].WhName);
        }

        [Fact]
        public void GetByOrganization_ShouldReturnCorrectWarehouses()
        {
            var service = CreateCleanService();
            var org1 = new Organization("Орг 1");
            var org2 = new Organization("Орг 2");

            var wh1 = new Warehouse("Склад 1", "Адрес 1", org1.OrgId);
            var wh2 = new Warehouse("Склад 2", "Адрес 2", org1.OrgId);
            var wh3 = new Warehouse("Склад 3", "Адрес 3", org2.OrgId);

            service.Warehouses.Add(wh1);
            service.Warehouses.Add(wh2);
            service.Warehouses.Add(wh3);

            var result = service.GetByOrganization(org1.OrgId);

            Assert.Equal(2, result.Count);
            Assert.All(result, w => Assert.Equal(org1.OrgId, w.OrgId));
        }

        [Fact]
        public void Delete_ShouldRemoveWarehouse()
        {
            var service = CreateCleanService();
            var org = new Organization("Тест");
            var warehouse = new Warehouse("Склад 1", "Адрес 1", org.OrgId);
            service.Warehouses.Add(warehouse);

            service.Warehouses.Remove(warehouse);

            Assert.Empty(service.Warehouses);
        }
    }

    public class ProductServiceTests
    {
        private ProductService CreateCleanService()
        {
            var service = new ProductService();
            service.Products.Clear();
            return service;
        }

        [Fact]
        public void Add_ShouldAddProduct()
        {
            var service = CreateCleanService();
            var product = new Product { Name = "Товар 1", Price = 100, Stock = 10 };

            service.Products.Add(product);

            Assert.Single(service.Products);
            Assert.Equal("Товар 1", service.Products[0].Name);
        }

        [Fact]
        public void GetByWarehouse_ShouldReturnCorrectProducts()
        {
            var service = CreateCleanService();
            var wh = new Warehouse { WhId = 1 };

            var p1 = new Product { Name = "Товар 1", WarehouseId = 1, Warehouse = wh };
            var p2 = new Product { Name = "Товар 2", WarehouseId = 1, Warehouse = wh };

            service.Products.Add(p1);
            service.Products.Add(p2);

            var result = service.GetByWarehouse(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Equal(1, p.WarehouseId));
        }

        [Fact]
        public void Delete_ShouldRemoveProduct()
        {
            var service = CreateCleanService();
            var product = new Product { Name = "Товар 1" };
            service.Products.Add(product);

            service.Products.Remove(product);

            Assert.Empty(service.Products);
        }
    }

    public class InvoiceServiceTests
    {
        [Fact]
        public void Add_ShouldAddInvoice()
        {
            var service = new InvoiceService();
            service.Invoices.Clear();
            var invoice = new Invoice { WarehouseId = 1, Type = InvoiceType.Incoming };

            service.Add(invoice);

            Assert.Single(service.Invoices);
        }

        [Fact]
        public void ProcessInvoice_Incoming_ShouldIncreaseStock()
        {
            var productService = new ProductService();
            productService.Products.Clear();
            var product = new Product { Id = 1, Name = "Товар", Price = 100, Stock = 10 };
            productService.Products.Add(product);

            var invoiceService = new InvoiceService();
            invoiceService.Invoices.Clear();
            var invoice = new Invoice { WarehouseId = 1, Type = InvoiceType.Incoming };
            invoice.Items.Add(new InvoiceItem { ProductId = 1, Product = product, Quantity = 5 });

            invoiceService.Add(invoice);
            invoiceService.ProcessInvoice(invoice, productService);

            Assert.Equal(15, product.Stock);
            Assert.True(invoice.IsCompleted);
        }

        [Fact]
        public void ProcessInvoice_Outgoing_ShouldDecreaseStock()
        {
            var productService = new ProductService();
            productService.Products.Clear();
            var product = new Product { Id = 1, Name = "Товар", Price = 100, Stock = 10 };
            productService.Products.Add(product);

            var invoiceService = new InvoiceService();
            invoiceService.Invoices.Clear();
            var invoice = new Invoice { WarehouseId = 1, Type = InvoiceType.Outgoing };
            invoice.Items.Add(new InvoiceItem { ProductId = 1, Product = product, Quantity = 3 });

            invoiceService.Add(invoice);
            invoiceService.ProcessInvoice(invoice, productService);

            Assert.Equal(7, product.Stock);
            Assert.True(invoice.IsCompleted);
        }

        [Fact]
        public void ProcessInvoice_Outgoing_ShouldNotGoBelowZero()
        {
            var productService = new ProductService();
            productService.Products.Clear();
            var product = new Product { Id = 1, Name = "Товар", Price = 100, Stock = 5 };
            productService.Products.Add(product);

            var invoiceService = new InvoiceService();
            invoiceService.Invoices.Clear();
            var invoice = new Invoice { WarehouseId = 1, Type = InvoiceType.Outgoing };
            invoice.Items.Add(new InvoiceItem { ProductId = 1, Product = product, Quantity = 10 });

            invoiceService.Add(invoice);
            invoiceService.ProcessInvoice(invoice, productService);

            Assert.Equal(0, product.Stock);
        }

        [Fact]
        public void GetByWarehouse_ShouldReturnCorrectInvoices()
        {
            var service = new InvoiceService();
            service.Invoices.Clear();
            var inv1 = new Invoice { WarehouseId = 1 };
            var inv2 = new Invoice { WarehouseId = 1 };
            var inv3 = new Invoice { WarehouseId = 2 };

            service.Add(inv1);
            service.Add(inv2);
            service.Add(inv3);

            var result = service.GetByWarehouse(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, i => Assert.Equal(1, i.WarehouseId));
        }
    }
}