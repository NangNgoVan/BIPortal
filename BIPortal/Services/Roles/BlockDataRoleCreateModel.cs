using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIPortal.Services.Roles
{
    public class BlockDataRoleCreateModel
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DeptId { get; set; }

        public List<int> ListRoleMenus { set; get; }
        public string StrAllowedMenus { get; set; }


        public BlockDataRoleCreateModel()
        {
            ListRoleMenus = new List<int>();
        }

    }
}