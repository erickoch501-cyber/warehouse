using System.Collections.ObjectModel;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class OrganizationService
    {
        public ObservableCollection<Organization> Organizations { get; set; }
            = new ObservableCollection<Organization>();

        public OrganizationService()
        {
            SeedData();
        }

        public void AddOrganization(string name)
        {
            Organizations.Add(new Organization(name));
        }

        public void EditOrganization(Organization org, string newName)
        {
            org.OrgName = newName;
        }

        public void DeleteOrganization(Organization org)
        {
            Organizations.Remove(org);
        }

        private void SeedData()
        {
            Organizations.Add(new Organization("DNS"));
            Organizations.Add(new Organization("М.Видео"));
            Organizations.Add(new Organization("Ситилинк"));
        }
    }
}