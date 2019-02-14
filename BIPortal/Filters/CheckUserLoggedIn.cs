using BIPortal.Controllers;
using BIPortal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace BIPortal.Filters
{
    public class CheckUserLoggedIn : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            try
            {
                CheckCurrentUserLoggedIn();
            }
            catch (Exception e)
            {

                var homeController = (BaseController)filterContext.Controller;
                filterContext.Result = homeController.RedirectToAction("Login", "Home", e.Message);
            }
        }

        private void CheckCurrentUserLoggedIn()
        {
            if (System.Web.HttpContext.Current.Session["username"] != null)
            {
                var currentUserName = System.Web.HttpContext.Current.Session["username"];
                var sessionId = System.Web.HttpContext.Current.Session.SessionID;

                if (!UserLoggedInStorage.checkIsUserLoggedIn((string)currentUserName, sessionId))
                {
                    throw new Exception("Tài khoản của bạn đã được đăng nhập ở một nơi khác!");
                }
            }
            else
            {
                throw new Exception("Bạn chưa đăng nhập! Vui lòng đăng nhập.");
            }
        }
    }

}
