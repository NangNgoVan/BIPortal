using System.Collections.Generic;

namespace BIPortal.Models.EntityModels
{
    public class EntityRoleModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DeptId { get; set; }
        public string OrganizationName { get; set; }

        public List<EntityUserRoleModel> listUserRole { get; set; }

        public List<EntityMenuModel> listtMenus { get; set; }
    }
}