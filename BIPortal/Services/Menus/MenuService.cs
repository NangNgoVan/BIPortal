using BIPortal.Models.EntityModels;
using BIPortal.Services.User;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BIPortal.Services.Menus
{
    public class MenuServices : DBBaseService
    {
        private UserServices _userService;

        private static string USP_GET_MENUIDS_BY_USERID = "usp_Get_MenuIds_By_UserId";
        private static string USP_GET_MENUS_BY_ORGID = "usp_Get_Menus_By_OrgId";
        private static string USP_GET_MENUS_BY_USERID = "usp_Get_Menus_By_UserId";

        public MenuServices(DBConnection dBConnection) : base(dBConnection)
        {
            _userService = new UserServices(dBConnection);
        }

        public int CreateMenu(EntityMenuModel menuModel)
        {
            int output = 0;

            try
            {
                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("DeptId", menuModel.DeptID);
                dicParas.Add("Name", menuModel.Name);
                dicParas.Add("Path", menuModel.Path);
                dicParas.Add("PathTableau", menuModel.PathTableau);
                dicParas.Add("Site_Root", menuModel.Site_Root);
                dicParas.Add("MenuLevel", menuModel.MenuLevel);
                dicParas.Add("ParentId", menuModel.ParentId);
                dicParas.Add("FilterCommand", menuModel.FilterCommand);
                dicParas.Add("FilterValue", menuModel.FilterValue);
                dicParas.Add("Status", menuModel.Status);
                dicParas.Add("Priority", menuModel.Priority);
                if (menuModel.MenuId == 0)
                    //dicParas.Add("")
                    output = DBConnection.ExecSPNonQuery("SP_MENU_INSERT", dicParas, ref dicParaOuts, true);
                else
                {
                    dicParas.Add("MenuId", menuModel.MenuId);
                    output = DBConnection.ExecSPNonQuery("SP_MENU_UPDATE", dicParas, ref dicParaOuts, true);
                }
                //STEP2:  ***************************************************************/
                if (DBConnection.ERROR != null)
                {
                    throw new Exception(DBConnection.ERROR);
                }
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return output;
        }

        public int Delete(int menuid)
        {
            int output = 0;

            try
            {
                DBConnection.OpenDBConnect();
                //STEP1:  ***************************************************************/

                Dictionary<string, object> dicParas = new Dictionary<string, object>();
                Dictionary<string, object> dicParaOuts = new Dictionary<string, object>();
                dicParas.Add("MENUID", menuid);

                //dicParas.Add("")
                output = DBConnection.ExecSPNonQuery("SP_MENU_DELETE", dicParas, ref dicParaOuts, true);

                //STEP2:  ***************************************************************/
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
                output = -1;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return output;
        }

        public EntityMenuModel GetMenuModel(string menuId)
        {
            EntityMenuModel output = new EntityMenuModel();
            DBConnection.OpenDBConnect();
            try
            {
                string sqlSelectMenu = " select * from Sys_Menu where menuid=@menuid";
                DBConnection.command.Parameters.Clear();
                DBConnection.command.CommandText = sqlSelectMenu;
                DBConnection.command.Parameters.AddWithValue("@menuid", int.Parse(menuId));

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            output.ParentId = reader.GetInt32(reader.GetOrdinal("ParentId"));
                            output.Path = reader.IsDBNull(reader.GetOrdinal("Path")) ? "" : reader.GetString(reader.GetOrdinal("Path")).Trim();
                            output.PathTableau = reader.IsDBNull(reader.GetOrdinal("PathTableau")) ? "" : reader.GetString(reader.GetOrdinal("PathTableau")).Trim();
                            output.Site_Root = reader.IsDBNull(reader.GetOrdinal("Site_Root")) ? "" : reader.GetString(reader.GetOrdinal("Site_Root"));
                            output.Name = reader.GetString(reader.GetOrdinal("Name"));
                            output.Priority = reader.GetInt32(reader.GetOrdinal("Priority"));
                            output.MenuId = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            output.DeptID = reader.GetInt32(reader.GetOrdinal("DeptId"));
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
            return output;
        }

        public IEnumerable<EntityMenuModel> GetMenusByDepId(int userId, int? deptId = null)
        {
            List<EntityMenuModel> data = new List<EntityMenuModel>();

            var currentUser = _userService.GetEntityById(userId);

            DBConnection.OpenDBConnect();
            DBConnection.command.Parameters.Clear();

            try
            {
                DBConnection.command.CommandType = System.Data.CommandType.StoredProcedure;

                if (currentUser.IsSuperAdmin)
                {
                    DBConnection.command.CommandText = USP_GET_MENUS_BY_ORGID;
                    DBConnection.command.Parameters.AddWithValue("@orgId", (object)deptId ?? DBNull.Value);
                }
                else if (currentUser.IsAdmin)
                {
                    DBConnection.command.CommandText = USP_GET_MENUS_BY_ORGID;
                    DBConnection.command.Parameters.AddWithValue("@orgId", currentUser.DeptID);
                }
                else
                {
                    DBConnection.command.CommandText = USP_GET_MENUS_BY_USERID;
                    DBConnection.command.Parameters.AddWithValue("@userId", userId);
                }
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityMenuModel entityMenuModel = new EntityMenuModel();
                            entityMenuModel.FilterCommand = reader.IsDBNull(reader.GetOrdinal("FilterCommand")) ? "" : reader.GetString(reader.GetOrdinal("FilterCommand"));
                            entityMenuModel.FilterValue = reader.IsDBNull(reader.GetOrdinal("FilterValue")) ? "" : reader.GetString(reader.GetOrdinal("FilterValue"));
                            entityMenuModel.LevelTree = reader.IsDBNull(reader.GetOrdinal("LevelTree")) ? "" : reader.GetString(reader.GetOrdinal("LevelTree"));
                            entityMenuModel.MenuId = reader.IsDBNull(reader.GetOrdinal("MenuId")) ? 0 : reader.GetInt32(reader.GetOrdinal("MenuId"));
                            entityMenuModel.MenuLevel = reader.IsDBNull(reader.GetOrdinal("MenuLevel")) ? "" : reader.GetString(reader.GetOrdinal("MenuLevel"));
                            entityMenuModel.Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "" : reader.GetString(reader.GetOrdinal("Name"));
                            entityMenuModel.ParentId = reader.IsDBNull(reader.GetOrdinal("ParentId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ParentId"));
                            entityMenuModel.Path = reader.IsDBNull(reader.GetOrdinal("Path")) ? "" : reader.GetString(reader.GetOrdinal("Path"));
                            entityMenuModel.Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? false : reader.GetBoolean(reader.GetOrdinal("Status"));
                            entityMenuModel.Priority = reader.IsDBNull(reader.GetOrdinal("Priority")) ? 0 : reader.GetInt32(reader.GetOrdinal("Priority"));
                            data.Add(entityMenuModel);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                // lỗi
                throw e;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }
            return data;
        }

        public EntityMenuModel FindByPathTabLeau(int menuId)
        {
            EntityMenuModel _pathTableau = null;
            this.DBConnection.OpenDBConnect();
            try
            {
                string sql = " select PathTableau,Site_Root from Sys_Menu where MenuId = @MenuId";
                this.DBConnection.command.Parameters.Clear();
                this.DBConnection.command.CommandText = sql;
                DBConnection.command.Parameters.AddWithValue("@MenuId", menuId);

                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            _pathTableau = new EntityMenuModel()
                            {
                                PathTableau = reader.IsDBNull(reader.GetOrdinal("PathTableau")) ? "" : reader.GetString(reader.GetOrdinal("PathTableau")).Trim(),
                                Site_Root = reader.IsDBNull(reader.GetOrdinal("Site_Root")) ? "" : reader.GetString(reader.GetOrdinal("Site_Root")).Trim()
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

            return _pathTableau;
        }

        // lấy danh sách quyền
        public List<int> GetUserMenusByUserId(int userId)
        {
            List<int> data = new List<int>();

            DBConnection.OpenDBConnect();
            DBConnection.command.Parameters.Clear();

            try
            {
                DBConnection.command.CommandText = USP_GET_MENUIDS_BY_USERID;
                DBConnection.command.CommandType = System.Data.CommandType.StoredProcedure;
                DBConnection.command.Parameters.AddWithValue("@userId", userId);
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data.Add(reader.GetInt32(reader.GetOrdinal("MenuId")));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return data;
        }

        // lấy danh sách quyền vs role
        public List<int> GetRoleMenusByRoleId(int roleId)
        {
            List<int> data = new List<int>();

            DBConnection.OpenDBConnect();
            DBConnection.command.Parameters.Clear();

            try
            {
                DBConnection.command.CommandText = $"SELECT [MenuId] FROM [dbo].[Sys_RoleMenu] where RoleId={roleId}";
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            data.Add(reader.GetInt32(reader.GetOrdinal("MenuId")));
                        }
                    }
                }
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }


            return data;
        }
    }
}