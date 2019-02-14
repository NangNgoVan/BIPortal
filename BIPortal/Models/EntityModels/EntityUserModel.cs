using System;
using System.Collections.Generic;

namespace BIPortal.Models.EntityModels
{
    public class EntityUserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}", ErrorMessage = "Password minimum 8 characters and contain at least one uppercase, one lowercase, one number.")]
        public string Password { get; set; }

        public string Salt { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }

        public int StatusID { get; set; }
        public string Status { get; set; }
        public int DeptID { set; get; }
        public string OrganizationCode { get; set; }
        public bool IsSuperAdmin { get; set; }

        public List<EntityUserRoleModel> LstUserRoles { get; set; }
        public List<EntityMenuModel> LstMenus { get; set; }

        public List<Int32> LstSelectedRole { get; set; }   // Danh sách các Role đã chọn từ select2
        public List<Int32> LstSelectedMenu { get; set; }
        public string OrgName { get; set; }


        public EntityUserModel()
        {
            LstMenus = new List<EntityMenuModel>();
            LstUserRoles = new List<EntityUserRoleModel>();
            LstSelectedRole = new List<int>();
            LstSelectedMenu = new List<int>();
        }
    }
}