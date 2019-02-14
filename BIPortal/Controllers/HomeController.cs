using BIPortal.Helpers;
using BIPortal.Models;
using BIPortal.Models.BusinessModels;
using BIPortal.Models.EntityModels;
using BIPortal.Models.UI;
using BIPortal.Services.SetDefaultPage;
using BIPortal.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BIPortal.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (Session[SESSION_NAME_USER_NAME] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                SetCommonData();


                GetLanguage();
                ViewData["VIEWDATA_LANGUAGE"] = LANGUAGE_OBJECT;

                BlockWelcomeLangModel blockWelcomeLang = new BlockWelcomeLangModel();
                Models.UI.BlockModel blockModel = new BlockModel("block_welcome", LANGUAGE_OBJECT, blockWelcomeLang);
                Logging.WriteToLog(Session[SESSION_CODE_ORGANIZATION] + "  " + Session[SESSION_NAME_USER_NAME] + " logged in.", LogType.Access);
                ViewData["BlockData"] = blockModel;
                return View("~/" + THEME_FOLDER + "/" + THEME_ACTIVE + "/index.cshtml");
            }
        }

        public ActionResult Logout()
        {

            System.Web.Security.FormsAuthentication.SignOut();
            Logging.WriteToLog("[Org] " + Session[SESSION_CODE_ORGANIZATION] + " [User Name] " + Session[SESSION_NAME_USER_NAME] + " logged out.", LogType.Access);
            Session.Clear();
            return RedirectToAction("Login");
        }


        [HttpGet]
        public ActionResult Login()
        {

            GetLanguage();
            ViewData["VIEWDATA_LANGUAGE"] = LANGUAGE_OBJECT;
            ViewData["LOGIN_FAIL_MESSAGE"] = TempData["MSG"];
            return View("~/" + THEME_FOLDER + "/" + THEME_ACTIVE + "/login.cshtml");
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            Char charRange = '/';
            //STEP1: GOI HAM LOGIN TOI CSDL *****************************************************
            this.SetConnectionDB();
            UserServices userServices = new UserServices(this.DBConnection);
            EntityUserModel entityUser = userServices.CheckLogin(loginModel);
            SetDefaultPageService setDefault = new SetDefaultPageService(DBConnection);

            var logInState = userServices.GetOrUpdateLoginState(loginModel.UserName, TIME_LOCK_ACCOUNT, false);

            if (logInState.StatusID == 1)
            {
                if (entityUser.UserName != null)
                {
                    Session[this.SESSION_NAME_USER_NAME] = entityUser.UserName;
                    Session[this.SESSION_NAME_USERID] = entityUser.UserId;
                    Session[SESSION_CODE_ORGANIZATION] = entityUser.OrganizationCode;
                    Session[SESSION_ID_ORGANIZATION] = entityUser.DeptID;
                    Session["IsAdmin"] = entityUser.IsAdmin;
                    Session["IsSuperAdmin"] = entityUser.IsSuperAdmin;
                    Session["OrganizationName"] = entityUser.OrgName;

                    List<EntityUserMenuModel> entityUserMenuModel = setDefault.GetListDefaultPage(entityUser.UserId);

                    UserLoggedInStorage.updateUserLoggedIn((string)Session[SESSION_NAME_USER_NAME], Session.SessionID);

                    userServices.GetOrUpdateLoginState(loginModel.UserName, TIME_LOCK_ACCOUNT, false, true);
                    foreach (EntityUserMenuModel item in entityUserMenuModel)
                    {
                        if (item.IsDefaultPage == true)
                        {
                            var _path = item.Path;
                            string _controller = _path.Split(charRange)[0];
                            string _action = _path.Split(charRange)[1];
                            int _menuId = item.MenuId;

                            return RedirectToAction(_action + "/" + _menuId, _controller);

                        }
                    }

                    return RedirectToAction("Index");
                }

                var logInStateIsUpdated = userServices.GetOrUpdateLoginState(loginModel.UserName, TIME_LOCK_ACCOUNT, true);

                if (logInStateIsUpdated.StatusID == 1)
                {
                    ViewData["LOGIN_FAIL_MESSAGE"] = $" Bạn đã đăng nhập sai {logInStateIsUpdated.LoginFailNo} lần. Nếu đăng nhập sai quá 5 lần, tài khoản sẽ bị khóa.";
                }
                else ViewData["LOGIN_FAIL_MESSAGE"] = $" Tài khoản của bạn đã bị khóa! Chờ 10 phút sau đăng nhập lại. ";
            }
            else if (logInState.StatusID == 0 && entityUser.UserName != null)
            {
                ViewData["LOGIN_FAIL_MESSAGE"] = $" Tài khoản đang tạm thời bị khóa! Vui lòng quay lại sau !";
            }
            else if (entityUser.UserName == null)
            {
                ViewData["LOGIN_FAIL_MESSAGE"] = $" Tên đăng nhập không chính xác !";
            }

            //if (userServices.ERROR != null) Session["msgcode"] = MessageType.ServerError;
            //else  Session["msgcode"] = MessageType.BusinessError;

            this.GetLanguage();
            ViewData["VIEWDATA_LANGUAGE"] = this.LANGUAGE_OBJECT;

            return View("~/" + this.THEME_FOLDER + "/" + this.THEME_ACTIVE + "/login.cshtml", loginModel);

            //STEP2: NEU DANG NHAP KHONG THANH CONG

        }

        public ActionResult ErrorPage()
        {
            return View();
        }


    }
}