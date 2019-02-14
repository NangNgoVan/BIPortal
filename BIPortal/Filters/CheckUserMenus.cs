using BIPortal.Models.EntityModels;
using BIPortal.Services;
using BIPortal.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace BIPortal.Filters
{


    public class CheckUserMenus : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //EICrypher eICrypher = new EICrypher();
            string connectString = WebConfigurationManager.AppSettings["CONNECTION_STRING"];
            //string connection = eICrypher.decryptString(connectString, "lT+iABLjC+D8nXSyqrq5RWxH54tlB8H5");
            //connectString = connection;
            DBConnection dBConnection = new DBConnection(connectString);
            var IDSession = filterContext.HttpContext.Session["session_userid"];
            var isAdmin = filterContext.HttpContext.Session["isAdmin"];
            int id = (IDSession != null) ? (int)IDSession : 0;
            UserServices userServices = new UserServices(dBConnection);
            Char charRange = '?';

            try
            {
                EntityUserModel currentUser = userServices.FindById(id);
                List<EntityMenuModel> userMenu = userServices.GetAllowedMenuAndRoles(currentUser.UserId);

                string _path = filterContext.HttpContext.Request.RawUrl;
                string _basePath = _path.Split(charRange)[0];

                bool hasPermission = false;
                if ((bool)isAdmin == true)
                {
                    hasPermission = true;
                }

                foreach (EntityMenuModel menu in userMenu)
                {
                    //if (_path.Contains( menu.Path))
                    if (menu.Path == _basePath)

                        hasPermission = true;

                }
                if (!hasPermission) throw new Exception();
            }
            catch (Exception ex)
            {

                filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary(new { controller = "Home", action = "Logout" })
                );
            }
        }



    }

}