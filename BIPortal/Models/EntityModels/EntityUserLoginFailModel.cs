using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIPortal.Models.EntityModels
{
    public class EntityUserLoginFailModel
    {
        public int? LoginFailNo { get; set; }
        public DateTime? LastLogin { get; set; }
        public int StatusID { get; set; }
    }
}