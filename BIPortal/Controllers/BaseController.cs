using BIPortal.Filters;
using BIPortal.Models.EntityModels;
using BIPortal.Models.UI;
using BIPortal.Services.Menus;
using BIPortal.Services.User;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace BIPortal.Controllers
{
    public class BaseController : Controller
    {
        public string SESSION_LANGUAGE_NAME { get; set; }
        public string LANGUAGE_FOLDER { get; set; }
        public string LANGUAGE { get; set; }
        public JObject LANGUAGE_OBJECT { get; set; }
        public string LOG_FOLDER { get; set; }
        public string LOG_FILE_ID { get; set; }
        public string CONFIG_FOLDER { get; set; }
        public string THEME_FOLDER { get; set; }
        public string THEME_ACTIVE { get; set; }
        public int TIME_LOCK_ACCOUNT { get; set; }
        /// <summary>
        /// used to set mode of the system is debug or not
        /// </summary>
        public bool IS_DEBUG { get; set; }
        public string ERRORS { get; set; }
        public string CONNECTION_STRING { get; set; }
        public string CONNECTION_STRING_DWH { get; set; }
        public string SESSION_NAME_USERID { get; set; }
        public string SESSION_NAME_USER_NAME { get; set; }
        public string SESSION_CODE_ORGANIZATION { get; set; }
        public string SESSION_ID_ORGANIZATION { get; set; }
        public Services.DBConnection DBConnection { get; set; }
        public string API_URL { get; set; }

        public BaseController()
        {
            SetProperty();
        }

        private void SetProperty()
        {
            CONNECTION_STRING = WebConfigurationManager.AppSettings["CONNECTION_STRING"];
            CONNECTION_STRING_DWH = WebConfigurationManager.AppSettings["CONNECTION_STRING_DWH"];
            //Connection Database Web
            //EICrypher eICrypher = new EICrypher();
            //CONNECTION_STRING = WebConfigurationManager.AppSettings["CONNECTION_STRING"];
            //string connectString = eICrypher.decryptString(CONNECTION_STRING, "lT+iABLjC+D8nXSyqrq5RWxH54tlB8H5");
            //CONNECTION_STRING = connectString;


            // Connection Database DWH
            //CONNECTION_STRING_DWH = WebConfigurationManager.AppSettings["CONNECTION_STRING_DWH"];
            //string connectStringDWH = eICrypher.decryptString(CONNECTION_STRING_DWH, "lT+iABLjC+D8nXSyqrq5RWxH54tlB8H5");
            //CONNECTION_STRING_DWH = connectStringDWH;


            SESSION_NAME_USERID = WebConfigurationManager.AppSettings["SESSION_NAME_USERID"];
            SESSION_NAME_USER_NAME = WebConfigurationManager.AppSettings["SESSION_NAME_USER_NAME"];

            SESSION_CODE_ORGANIZATION = WebConfigurationManager.AppSettings["SESSION_CODE_ORGANIZATION"];
            SESSION_ID_ORGANIZATION = WebConfigurationManager.AppSettings["SESSION_ID_ORGANIZATION"];
            LOG_FOLDER = WebConfigurationManager.AppSettings["LOG_FOLDER"];
            THEME_ACTIVE = WebConfigurationManager.AppSettings["THEME_ACTIVE"];
            THEME_FOLDER = WebConfigurationManager.AppSettings["THEME_FOLDER"];

            SESSION_LANGUAGE_NAME = WebConfigurationManager.AppSettings["SESSION_LANGUAGE_NAME"];
            LANGUAGE_FOLDER = WebConfigurationManager.AppSettings["LANGUAGE_FOLDER"];

            CONFIG_FOLDER = WebConfigurationManager.AppSettings["CONFIG_FOLDER"];
            TIME_LOCK_ACCOUNT = int.Parse(WebConfigurationManager.AppSettings["TIME_LOCK_ACCOUNT"]);
            API_URL = WebConfigurationManager.AppSettings["apiUrl"];
        }

        public void GetLanguage()
        {
            if (Session[SESSION_LANGUAGE_NAME] == null)
            {
                LANGUAGE = "vi";
                Session[SESSION_LANGUAGE_NAME] = LANGUAGE;

            }
            else LANGUAGE = Session[SESSION_LANGUAGE_NAME].ToString();

            LANGUAGE_OBJECT = Helpers.Utility.JTokenHelper.GetLanguage("~/" + LANGUAGE_FOLDER, LANGUAGE);
            ViewData["VIEWDATA_LANGUAGE"] = LANGUAGE_OBJECT;
        }

        public void SetConnectionDB()
        {
            DBConnection = new Services.DBConnection(CONNECTION_STRING);
        }

        /// <summary>
        /// GET THE DATA USE FOR COMMON TARGET AS MENUS,....
        /// </summary>
        public void SetCommonData()
        {
            this.SetConnectionDB();
            UserServices userServices = new UserServices(DBConnection);

            EntityUserModel currentUser = userServices.GetEntityById((int)Session[SESSION_NAME_USERID]);
            //ViewData["block_menu_left_data"] = userServices.GetListMenus(currentUser);

            MenuServices menuServices = new MenuServices(DBConnection);

            var menuData = menuServices.GetMenusByDepId(currentUser.UserId, currentUser.DeptID);

            ViewData["MenuHeaderData"] = menuData;
        }


        public void SetMessageText(BlockLanguageModel blockLanguage)
        {
            //string output = null;
            try
            {
                if (Session["msgtype"] != null)
                {
                    int msgType = Int32.Parse(Session["msgtype"].ToString());
                    ViewBag.Message = blockLanguage.GetMessage(msgType);

                    Session["msgtype"] = null;
                    Session.Remove("msgtype");
                }
            }
            catch (Exception ex)
            {

            }
            //return output;
        }

        public ActionResult CheckPermission()
        {


            bool hasPermission = true;

            if (!hasPermission)
            {
                return RedirectToAction("Home", "Logout");
            }
            return Content("");
        }

        public ActionResult CheckAdminPermission()
        {

            if (Session["IsAdmin"] != null || (bool)Session["IsAdmin"] == true)
            {
                return RedirectToAction("Logout", "Home");
                //RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
                //RedirectToAction("Home", "Login", routeValueDictionary);
                //Server.Transfer("Home/Logout");
                //return Content("<script>window.location = 'http://www.example.com';</script>");
                //return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Home" }));
            }
            return Content("");
        }

        public RedirectToRouteResult RedirectToAction(string action, string controller, string msg)
        {
            TempData["MSG"] = msg;
            return RedirectToAction(action, controller);
        }

    }
}
