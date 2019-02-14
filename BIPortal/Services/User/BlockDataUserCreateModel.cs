using BIPortal.Models;
using BIPortal.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIPortal.Services.User
{
    public class BlockDataUserCreateModel : EntityUserModel
    {
        public List<EntityRoleModel> ListAllRoles { set; get; }

        public string StrAllowedMenus { get; set; }
        public BlockDataUserCreateModel() : base()
        {
            ListAllRoles = new List<EntityRoleModel>();
        }
    }
}