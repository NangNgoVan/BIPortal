using BIPortal.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BIPortal.Services.SetDefaultPage
{
    public class SetDefaultPageService : DBBaseService
    {
        public SetDefaultPageService(DBConnection dBConnection) : base(dBConnection)
        {

        }
        private string usp_Get_DefaultPage = "usp_Get_DefaultPage";
        private string usp_Update_IsDefaultPage = "usp_Update_IsDefaultPage";
        private string usp_UpdatePage = "usp_UpdatePage";
        public List<EntityUserMenuModel> GetListDefaultPage(int userId)
        {
            List<EntityUserMenuModel> _listAllDefaultPage = new List<EntityUserMenuModel>();
            DBConnection.OpenDBConnect();
            try
            {

                //string sqlSelectListDefaultPage = "SELECT  UserId, a.[MenuId],b.Path,b.Name,[IsDefaultPage] FROM [SUN_WEBSITE_UAT].[dbo].[Sys_UserMenu] a inner join [dbo].[Sys_Menu] b ON a.MenuId = b.MenuId and b.Path is not null and UserId = @UserId";

                //DBConnection.command.CommandText = sqlSelectListDefaultPage;
                DBConnection.command.CommandText = usp_Get_DefaultPage;
                DBConnection.command.CommandType = CommandType.StoredProcedure;

                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                using (SqlDataReader reader = DBConnection.command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EntityUserMenuModel entityUserMenu = new EntityUserMenuModel();
                            entityUserMenu.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                            entityUserMenu.MenuId = reader.GetInt32(reader.GetOrdinal("MenuId"));
                            entityUserMenu.Path = reader.GetString(reader.GetOrdinal("Path")).ToString().Trim();
                            entityUserMenu.Name = reader.GetString(reader.GetOrdinal("Name")).ToString().Trim();
                            entityUserMenu.IsDefaultPage = reader.IsDBNull(reader.GetOrdinal("IsDefaultPage")) ? false : reader.GetBoolean(reader.GetOrdinal("IsDefaultPage"));

                            _listAllDefaultPage.Add(entityUserMenu);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //string Error = ex.ToString();
                throw ex;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }

            return _listAllDefaultPage;
        }

        // Update IsDefaultPage = true
        public void UpdatePageDefault(int userId, int? menuId)
        {
            DBConnection.OpenDBConnect();
            try
            {
                bool isDefault = true;
                //string sqlUpdateIsDefaultPage = "Update Sys_UserMenu set IsDefaultPage = 'false' Where UserId =@UserId";
                //DBConnection.command.CommandText = sqlUpdateIsDefaultPage;
                DBConnection.command.CommandText = usp_Update_IsDefaultPage;
                DBConnection.command.CommandType = CommandType.StoredProcedure;

                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                DBConnection.command.ExecuteNonQuery();
                DBConnection.command.Parameters.Clear();

                //string sqlUpdatePage = "Update  Sys_UserMenu set IsDefaultPage= @IsDefaultPage where MenuId = @MenuId and UserId = @UserId";
                //DBConnection.command.CommandText = sqlUpdatePage;
                DBConnection.command.CommandText = usp_UpdatePage;
                DBConnection.command.CommandType = CommandType.StoredProcedure;

                DBConnection.command.Parameters.AddWithValue("@UserId", userId);
                DBConnection.command.Parameters.AddWithValue("@MenuId", (object)menuId ?? DBNull.Value);


                DBConnection.command.Parameters.AddWithValue("@IsDefaultPage", isDefault);

                DBConnection.command.ExecuteNonQuery();
                DBConnection.command.Parameters.Clear();
            }

            catch (Exception ex)
            {
                //string Error = ex.ToString();
                throw ex;
            }
            finally
            {
                DBConnection.CloseDBConnect();
            }
        }

    }

}
