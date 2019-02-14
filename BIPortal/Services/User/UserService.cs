using BIPortal.Filters;
using BIPortal.Helpers.Security;
using BIPortal.Models;
using BIPortal.Models.EntityModels;
using BIPortal.Services.Roles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace BIPortal.Services.User
{

    public class UserServices : DBBaseService
    {
        private static string USP_GET_ALL_USERS = "usp_Get_All_Users";
        private static string USP_GET_MENUS_BY_USERID = "usp_Get_Menus_By_UserId";
        private static string USP_GET_USER_BY_ID = "usp_Get_User_By_Id";
        private static string USP_GET_USER_BY_USERNAME = "usp_Get_User_By_Username";
        private static string USP_UPDATE_LOGIN_STATE_BY_USERNAME = "usp_Update_Login_State_By_Username";
        private static string USP_GET_ALL_MENUS = "usp_Get_All_Menus";
        //private static string USP_GET_MENUIDS_BY_USERID = "usp_Get_MenuIds_By_UserId";
        private static string USP_GET_SALF_BY_USERID = "usp_Get_Salf_By_UserId";
        private static string USP_CHECK_USER_LOG_IN = "usp_Check_User_Log_In";

        public UserServices(DBConnection dBConnection) : base(dBConnection)
        {

        }

        public EntityUserModel CheckLogin(LoginModel loginModel)
        {
            EntityUserModel output = new EntityUserModel();
            EntityStatusModel status = new EntityStatusModel();
            bool isNotLDAP = true;
            //string sqlUser = "";
            try
            {


                string passwordHashed = "";
                var _salt = string.Empty;
                //sqlUser = Utilities.IsAuthenticated(loginModel.UserName, loginModel.Password);

                //if (sqlUser == "")
                //{
                //    isNotLDAP = false;
                //}

                PasswordManager pwm = new PasswordManager();

                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/
                //******************BAM MAT KHAU THEO SALT (CO TRONG CSDL) VA MAT KHAU DUOC NHAP VAO ***********/
                DBConnection.command.Parameters.AddWithValue("@UserName", loginModel.UserName);
                DBConnection.command.CommandText = USP_GET_SALF_BY_USERID;//sqlUserSalt;
                DBConnection.command.CommandType = CommandType.StoredProcedure;
                _salt = DBConnection.command.ExecuteScalar() as string;

                passwordHashed = pwm.IsMatch(loginModel.Password, _salt);

                //STEP2:  ***************************************************************/
                DBConnection.command.Parameters.Clear();
                //DBConnection.command.CommandText = sqlUser;
                DBConnection.command.CommandText = USP_CHECK_USER_LOG_IN;
                DBConnection.command.CommandType = CommandType.StoredProcedure;

                DBConnection.command.Parameters.AddWithValue("@UserName", loginModel.UserName);
                if (isNotLDAP)
                {
                    DBConnection.command.Parameters.AddWithValue("@passWord", passwordHashed);
                }
                else
                {
                    DBConnection.command.Parameters.AddWithValue("@passWord", DBNull.Value);
                }


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            output.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            output.UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName"));
                            output.Salt = reader.IsDBNull(reader.GetOrdinal("Salt")) ? "" : reader.GetString(reader.GetOrdinal("Salt"));
                            output.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email"));
                            output.Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString(reader.GetOrdinal("Phone"));
                            output.IsAdmin = reader.GetBoolean(reader.GetOrdinal("IsAdmin"));
                            output.OrganizationCode = reader.IsDBNull(reader.GetOrdinal("Code")) ? "" : reader.GetString(reader.GetOrdinal("Code"));
                            output.StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"));
                            output.DeptID = reader.GetInt32(reader.GetOrdinal("deptID"));
                            output.OrgName = reader.IsDBNull(reader.GetOrdinal("OrgName")) ? "" : reader.GetString(reader.GetOrdinal("OrgName"));
                        }
                    }
                }

                output.IsSuperAdmin = output.IsAdmin && (output.DeptID == 0);
            }

            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                throw ex;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            //if (output.StatusID != 1) return null;
            return output;
        }

        //public BlockDataMenuLeftModel GetListMenus(int userId, bool isAdmin)
        //{
        //    BlockDataMenuLeftModel output = new BlockDataMenuLeftModel();


        //    try
        //    {
        //        EntityUserModel entityUser = new EntityUserModel();

        //        entityUser.UserId = userId;
        //        entityUser.IsAdmin = isAdmin;

        //        output = this.GetListMenus(entityUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.ERROR = ex.ToString();
        //        this.DBConnection.CloseDBConnect();
        //    }
        //    finally
        //    {
        //        DBConnection.CloseDBConnect();
        //    }
        //    return output;
        //}
        //public BlockDataMenuLeftModel GetListMenus(EntityUserModel entityUser)
        //{
        //    BlockDataMenuLeftModel output = new BlockDataMenuLeftModel();

        //    this.DBConnection.OpenDBConnect();
        //    output.EntityUserModel = entityUser;
        //    try
        //    {
        //        this.DBConnection.command.CommandText = USP_GET_MENUS_BY_USERID;
        //        this.DBConnection.command.CommandType = CommandType.StoredProcedure;
        //        this.DBConnection.command.Parameters.AddWithValue("@userId", entityUser.UserId);

        //        using (SqlDataReader reader = DBConnection.command.ExecuteReader())
        //        {
        //            if (reader.HasRows)
        //            {
        //                while (reader.Read())
        //                {
        //                    int menuid = reader.GetInt32(reader.GetOrdinal("MenuId"));
        //                    if (!entityUser.LstSelectedMenu.Contains(menuid))
        //                        entityUser.LstSelectedMenu.Add(menuid);
        //                }

        //            }
        //        }

        //        this.DBConnection.command.CommandText = USP_GET_ALL_MENUS;
        //        DBConnection.command.CommandType = CommandType.StoredProcedure;

        //        List<EntityMenuModel> lstMenuTemp = new List<EntityMenuModel>();
        //        using (SqlDataReader reader = DBConnection.command.ExecuteReader())
        //        {
        //            if (reader.HasRows)
        //            {
        //                while (reader.Read())
        //                {
        //                    EntityMenuModel entityMenuModel = new EntityMenuModel();
        //                    entityMenuModel.FilterCommand = reader.IsDBNull(reader.GetOrdinal("FilterCommand")) ? "" : reader.GetString(reader.GetOrdinal("FilterCommand"));
        //                    entityMenuModel.FilterValue = reader.IsDBNull(reader.GetOrdinal("FilterValue")) ? "" : reader.GetString(reader.GetOrdinal("FilterValue"));
        //                    entityMenuModel.LevelTree = reader.IsDBNull(reader.GetOrdinal("LevelTree")) ? "" : reader.GetString(reader.GetOrdinal("LevelTree"));
        //                    entityMenuModel.MenuId = reader.IsDBNull(reader.GetOrdinal("MenuId")) ? 0 : reader.GetInt32(reader.GetOrdinal("MenuId"));
        //                    entityMenuModel.MenuLevel = reader.IsDBNull(reader.GetOrdinal("MenuLevel")) ? "" : reader.GetString(reader.GetOrdinal("MenuLevel"));
        //                    entityMenuModel.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name"));
        //                    entityMenuModel.ParentId = reader.IsDBNull(reader.GetOrdinal("Name")) ? 0 : reader.GetInt32(reader.GetOrdinal("ParentId"));
        //                    entityMenuModel.Path = reader.IsDBNull(reader.GetOrdinal("Path")) ? "" : reader.GetString(reader.GetOrdinal("Path"));
        //                    entityMenuModel.Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? false : reader.GetBoolean(reader.GetOrdinal("Status"));
        //                    entityMenuModel.Priority = reader.GetInt32(reader.GetOrdinal("Priority"));

        //                    lstMenuTemp.Add(entityMenuModel);
        //                    if (entityUser.IsAdmin == true)
        //                    {
        //                        entityUser.LstSelectedMenu.Add(entityMenuModel.MenuId);
        //                        output.StrAllowedLeveltrees += entityMenuModel.LevelTree + ",";
        //                        output.StrAllowedMenuIds += entityMenuModel.MenuId + ",";
        //                        continue;
        //                    }
        //                    if (entityUser.LstSelectedMenu.Contains(entityMenuModel.MenuId))
        //                    {
        //                        // output.LstAllowedMenus.Add(entityMenuModel.Clone());
        //                        output.StrAllowedLeveltrees += entityMenuModel.LevelTree + ",";
        //                        output.StrAllowedMenuIds += entityMenuModel.MenuId + ",";
        //                    }
        //                }

        //            }
        //        }
        //        output.StrAllowedMenuIds = "," + output.StrAllowedMenuIds;
        //        output.StrAllowedLeveltrees = "," + output.StrAllowedLeveltrees;
        //        foreach (EntityMenuModel entity in lstMenuTemp)
        //        {
        //            if (entityUser.LstSelectedMenu.Contains(entity.MenuId))
        //            {
        //                //output.LstAllowedMenus.Add(entity.Clone());
        //                //output.LstAllOfMenus.Add(entity.Clone());
        //                continue;
        //            }

        //            else
        //            {
        //                string currentMenuLeveltree = entity.LevelTree + "@@@";
        //                bool added = false;
        //                while (currentMenuLeveltree.Length > 0)
        //                {
        //                    int pos = currentMenuLeveltree.LastIndexOf("@@@");
        //                    if (pos < 0) break;
        //                    currentMenuLeveltree = currentMenuLeveltree.Substring(0, pos);

        //                    //DAY LA TRUONG HOP CO MENU CHA NAM TRONG SO DUOC PHAN QUYEN
        //                    //THI ADD MENU HIEN TAI VAO NHOM 
        //                    if (output.StrAllowedLeveltrees.IndexOf("," + currentMenuLeveltree + ",") >= 0)
        //                    {
        //                        output.StrAllowedLeveltrees += entity.LevelTree + ",";
        //                        output.StrAllowedMenuIds += entity.MenuId.ToString() + ",";
        //                        entityUser.LstSelectedMenu.Add(entity.MenuId);
        //                        added = true;
        //                        break;
        //                    }

        //                }


        //            }
        //        }

        //        foreach (EntityMenuModel entity in lstMenuTemp)
        //        {
        //            if (entityUser.IsAdmin == true)
        //            {
        //                output.LstAllOfMenus.Add(entity.Clone());
        //                continue;
        //            }

        //            if (entityUser.LstSelectedMenu.Contains(entity.MenuId))
        //            {
        //                output.LstAllOfMenus.Add(entity.Clone());
        //            }

        //            else
        //            {
        //                //KIEM TRA MENU HIEN TAI CO LA MENU CHA CUA 1 TRONG SO CAC MENU DA DUOC ADD KO
        //                if (output.StrAllowedLeveltrees.Contains("," + entity.LevelTree + "@"))
        //                    output.LstAllOfMenus.Add(entity.Clone());
        //            }

        //        }

        //        //STEP3: GET LIST OF ALLOWED MENU FOR THE CURRENT USER
        //    }
        //    catch (Exception ex)
        //    {
        //        this.ERROR = ex.ToString();
        //    }



        //    return output;
        //}
        public int Create(BlockDataUserCreateModel model)
        {
            int output = 0;
            string _salt = "";
            PasswordManager pwm = new PasswordManager();
            try
            {


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParaOuts.Add("result", "");
                string LstOfMenuIds = model.StrAllowedMenus;
                string LstOfRoleIds = string.Join(",", model.LstSelectedRole);
                if (model.Password != null)
                {
                    string passwordHashed = pwm.GetPasswordHashedAndGetSalt(model.Password, out _salt);
                    model.Password = passwordHashed;
                    model.Salt = _salt;

                }
                var getusername = GetUsername();

                // superadmin
                if (model.IsSuperAdmin)
                {
                    model.IsAdmin = true;
                    model.DeptID = 0;
                }

                if (model.UserId == 0)
                {
                    if (getusername.Where(x => x.UserName == model.UserName).FirstOrDefault() != null)
                    {
                        throw new Exception("Tên đăng nhập đã tồn tại");

                    }
                    else
                    {
                        dicParas.Add("UserId", model.UserId);
                        dicParas.Add("UserName", model.UserName);
                        dicParas.Add("Password", model.Password);
                        dicParas.Add("Salt", model.Salt);
                        dicParas.Add("Email", model.Email);
                        dicParas.Add("Phone", model.Phone);
                        dicParas.Add("IsAdmin", model.IsAdmin);
                        dicParas.Add("deptID", model.DeptID);
                        dicParas.Add("StatusID", model.StatusID);
                        dicParas.Add("LstOfMenuIds", model.StrAllowedMenus);
                        dicParas.Add("LstOfRoleIds", LstOfRoleIds);
                        output = DBConnection.ExecSPNonQuery("SP_USER_INSERT_OR_UPDATE", dicParas, ref dicParaOuts, true);
                        if (DBConnection.ERROR != null)
                            throw new Exception(DBConnection.ERROR);
                    }
                }
                else
                {
                    if (getusername.Where(x => x.UserId != model.UserId && x.UserName == model.UserName).FirstOrDefault() != null)
                    {
                        throw new Exception("Tên đăng nhập đã tồn tại");
                    }
                    if (getusername.Where(x => x.UserId == model.UserId && x.UserName == model.UserName).FirstOrDefault() != null || getusername.Where(x => x.UserId == model.UserId && x.UserName != model.UserName).FirstOrDefault() != null)
                    {
                        dicParas.Add("UserId", model.UserId);
                        dicParas.Add("UserName", model.UserName);
                        dicParas.Add("Password", model.Password);
                        dicParas.Add("Salt", model.Salt);
                        dicParas.Add("Email", model.Email);
                        dicParas.Add("Phone", model.Phone);
                        dicParas.Add("IsAdmin", model.IsAdmin);
                        dicParas.Add("deptID", model.DeptID);
                        dicParas.Add("StatusID", model.StatusID);
                        dicParas.Add("LstOfMenuIds", model.StrAllowedMenus);
                        dicParas.Add("LstOfRoleIds", LstOfRoleIds);
                        output = DBConnection.ExecSPNonQuery("SP_USER_INSERT_OR_UPDATE", dicParas, ref dicParaOuts, true);
                        if (DBConnection.ERROR != null)
                            throw new Exception(DBConnection.ERROR);
                    }


                }

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                if (this.IsCloseDBAfterExecute) this.DBConnection.CloseDBConnect();
            }

            return output;
        }



        public List<EntityUserModel> GetList(int? depId = null)
        {
            List<EntityUserModel> output = new List<EntityUserModel>();

            this.DBConnection.OpenDBConnect();
            try
            {
                this.DBConnection.command.CommandText = USP_GET_ALL_USERS;
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;


                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityUserModel entity = new EntityUserModel();
                            entity.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            entity.UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName"));
                            entity.Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? "" : reader.GetString(reader.GetOrdinal("Phone"));
                            entity.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email"));
                            entity.Status = reader.GetString(reader.GetOrdinal("Status"));
                            entity.OrganizationCode = reader.IsDBNull(reader.GetOrdinal("code")) ? "" : reader.GetString(reader.GetOrdinal("Code"));
                            entity.DeptID = reader.GetInt32(reader.GetOrdinal("deptID"));
                            output.Add(entity);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }


            return depId == null ? output : output.Where(x => x.DeptID == depId).ToList();
        }

        public BlockDataUserCreateModel GetEntityById(int id)
        {
            BlockDataUserCreateModel output = new BlockDataUserCreateModel();
            try
            {
                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/


                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("USERID", id);
                DataSet dataSet = DBConnection.ExecSelectSP("SP_USER_GET_BY_ID", dicParas, ref dicParaOuts, true);
                //**********************TABLE: ROLE***************************************
                DataTable table1 = dataSet.Tables[0];
                foreach (DataRow row in table1.Rows)
                {
                    output.Email = row["Email"].ToString();
                    output.Phone = row["Phone"].ToString();
                    output.IsAdmin = (bool)row["IsAdmin"];
                    output.UserName = (string)row["UserName"];
                    output.UserId = (int)row["UserId"];
                    output.DeptID = (int)row["DeptID"];
                    output.StatusID = (int)row["StatusID"];
                    output.OrganizationCode = row["Code"].ToString().Trim();
                }

                output.IsSuperAdmin = output.IsAdmin && (output.DeptID == 0);

                //**********************TABLE: ROLEMENU ***********************************************
                DataTable table2 = dataSet.Tables[1];

                foreach (DataRow row in table2.Rows)
                {
                    output.LstSelectedMenu.Add(Int32.Parse(row["menuid"].ToString()));
                }

                foreach (DataRow row in dataSet.Tables[2].Rows)
                {
                    output.LstSelectedRole.Add(Int32.Parse(row["roleid"].ToString()));
                }
                //**********************TABLE: ROLE ***********************************************
                DataTable table3 = dataSet.Tables[2];
                foreach (DataRow row in table3.Rows)
                {
                    BIPortal.Models.EntityModels.EntityRoleModel entityRoleModel = new EntityRoleModel();
                    entityRoleModel.Name = row["Name"].ToString();
                    entityRoleModel.RoleId = (int)row["RoleId"];
                    output.ListAllRoles.Add(entityRoleModel);
                }
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return output;
        }

        public int Delete(int id)
        {
            int output = 0;

            try
            {

                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/
                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("userid", id);

                //dicParas.Add("")
                output = DBConnection.ExecSPNonQuery("SP_USER_DELETE", dicParas, ref dicParaOuts, true);

                //STEP2:  ***************************************************************/

            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }


            return output;
        }

        public List<EntityMenuModel> GetAllowedMenuAndRoles(int userId)
        {
            List<EntityMenuModel> _listAllowedAndRoles = new List<EntityMenuModel>();
            DBConnection.OpenDBConnect();
            try
            {
                DBConnection.command.CommandText = USP_GET_MENUS_BY_USERID;
                DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@UserId", userId);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityMenuModel entityMenu = new EntityMenuModel();
                            entityMenu.MenuId = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            entityMenu.Path = reader.IsDBNull(reader.GetOrdinal("Path")) ? "" : "/" + reader.GetString(reader.GetOrdinal("Path")).Trim() + "/" + reader.GetInt32(reader.GetOrdinal("MenuId")).ToString().Trim();
                            entityMenu.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name")).Trim();
                            _listAllowedAndRoles.Add(entityMenu);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }
            return _listAllowedAndRoles;
        }

        public EntityUserModel FindById(int userId)
        {
            EntityUserModel _user = null;
            this.DBConnection.OpenDBConnect();
            try
            {
                this.DBConnection.command.CommandText = USP_GET_USER_BY_ID;
                DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@userID", userId);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            _user = new EntityUserModel()
                            {
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? false : reader.GetBoolean(reader.GetOrdinal("IsAdmin"))

                            };

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }

            return _user;
        }

        public EntityUserLoginFailModel GetOrUpdateLoginState(string userName, int timeLockAccount, bool isUpdateWhenLoginFail = false, bool isReset = false)

        {
            var LoginFailInfo = new EntityUserLoginFailModel(); ;

            DBConnection.OpenDBConnect();
            try
            {
                DBConnection.command.CommandText = USP_GET_USER_BY_USERNAME;
                DBConnection.command.CommandType = CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@userName", userName);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LoginFailInfo.LoginFailNo = reader.IsDBNull(reader.GetOrdinal("LoginFailNo")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("LoginFailNo"));
                        LoginFailInfo.LastLogin = reader.IsDBNull(reader.GetOrdinal("LastTimeLogin")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastTimeLogin"));
                        LoginFailInfo.StatusID = reader.GetInt32(reader.GetOrdinal("StatusID"));
                    }
                }

                if (isUpdateWhenLoginFail || isReset)
                {
                    DBConnection.command.Parameters.Clear();
                    DBConnection.command.CommandText = USP_UPDATE_LOGIN_STATE_BY_USERNAME;
                    DBConnection.command.CommandType = CommandType.StoredProcedure;
                    DBConnection.command.Parameters.AddWithValue("@userName", userName);

                    int loginFailNo = 0;
                    DateTime? lastTimeLogin = DateTime.Now;
                    int statusID = LoginFailInfo.StatusID;

                    var currentLoginFailNo = LoginFailInfo.LoginFailNo == null ? 0 : LoginFailInfo.LoginFailNo.Value;
                    var currentLastTimeLogin = LoginFailInfo.LastLogin == null ? lastTimeLogin : LoginFailInfo.LastLogin.Value;

                    int diff = (int)(lastTimeLogin - currentLastTimeLogin).Value.TotalMinutes;

                    if (diff >= timeLockAccount) isReset = true;

                    if (!isReset)
                    {
                        loginFailNo = isUpdateWhenLoginFail ? currentLoginFailNo + 1 : currentLoginFailNo;

                        if (loginFailNo == 5)
                        {
                            statusID = 0;
                        }
                    }
                    else
                    {
                        loginFailNo = 0;
                        statusID = 1;
                        lastTimeLogin = null;
                    }

                    DBConnection.command.Parameters.AddWithValue("@LoginFailNo", (object)loginFailNo ?? DBNull.Value);
                    DBConnection.command.Parameters.AddWithValue("@LastTimeLogin", (object)lastTimeLogin ?? DBNull.Value);
                    DBConnection.command.Parameters.AddWithValue("@StatusID", (object)statusID ?? DBNull.Value);
                    DBConnection.command.ExecuteNonQuery();
                    DBConnection.command.Parameters.Clear();

                    return new EntityUserLoginFailModel
                    {
                        LoginFailNo = loginFailNo,
                        LastLogin = lastTimeLogin,
                        StatusID = statusID
                    };
                }
                else
                {
                    return LoginFailInfo;
                }
            }

            finally
            {
                DBConnection.CloseDBConnect();
            }
        }

        public List<EntityGetUserModel> GetUsername()
        {
            DBConnection.OpenDBConnect();
            var model = new List<EntityGetUserModel>();
            try
            {
                this.DBConnection.command.CommandText = USP_GET_ALL_USERS;
                this.DBConnection.command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var user = new EntityGetUserModel();
                            user.UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? "" : reader.GetString(reader.GetOrdinal("UserName"));
                            user.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            model.Add(user);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string Error = ex.ToString();
            }
            finally
            {
                this.DBConnection.CloseDBConnect();
            }
            return model;
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ChangePw(BlockDataUserChangepwModel model)
        {
            int output = 0;
            string _oldsalt = "", newSalt = "";
            PasswordManager pwm = new PasswordManager();
            try
            {
                DBConnection.OpenDBConnect();
                if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");

                _oldsalt = string.Empty;

                string sqlUserSalt = "Select Salt from Users Where UserId = @Userid ";

                //STEP1:  ***************************************************************/
                //******************BAM MAT KHAU THEO SALT (CO TRONG CSDL) VA MAT KHAU DUOC NHAP VAO ***********/
                DBConnection.command.Parameters.AddWithValue("@Userid", model.UserId);
                DBConnection.command.CommandText = sqlUserSalt;
                _oldsalt = DBConnection.command.ExecuteScalar() as string;

                model.OldPassword = pwm.IsMatch(model.OldPassword, _oldsalt);

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParaOuts.Add("IsChanged", 0);

                model.Password = pwm.GetPasswordHashedAndGetSalt(model.Password, out newSalt);


                dicParas.Add("UserId", model.UserId);
                dicParas.Add("OldPassword", model.OldPassword);

                dicParas.Add("OldSalt", _oldsalt);
                dicParas.Add("NewPassword", model.Password);
                dicParas.Add("NewSalt", newSalt);

                output = DBConnection.ExecSPNonQuery("SP_USER_CHANGE_PASSWORD", dicParas, ref dicParaOuts, true);
                output = (int)dicParaOuts["IsChanged"];
            }
            catch (Exception ex)
            {
                this.ERROR = ex.ToString();
            }

            return output;
        }

        //public List<EntityUserModel> GetList()
        //{
        //    List<EntityUserModel> output = new List<EntityUserModel>();

        //    this.DBConnection.OpenDBConnect();
        //    //Write log
        //    if (this.DBConnection.ERROR != null) throw new Exception("Can't connect to db");
        //    try
        //    {
        //        string sql = " select UserId,UserName,Phone,Email from Sys_Users ";
        //        this.DBConnection.command.Parameters.Clear();
        //        this.DBConnection.command.CommandText = sql;


        //        using (SqlDataReader reader = DBConnection.command.ExecuteReader())
        //        {
        //            if (reader.HasRows)
        //            {
        //                while (reader.Read())
        //                {
        //                    EntityUserModel entity = new EntityUserModel();
        //                    entity.UserId =  reader.GetInt32(reader.GetOrdinal("UserId"));
        //                    entity.UserName = reader.GetString(reader.GetOrdinal("UserName"));
        //                    entity.Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone"));
        //                    entity.Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null :reader.GetString(reader.GetOrdinal("Email"));
        //                    output.Add(entity);
        //                }
        //            }
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        this.ERROR = ex.ToString();
        //    }
        //    finally
        //    {
        //        this.DBConnection.CloseDBConnect();
        //    }


        //    return output;
        //}


    }
}
