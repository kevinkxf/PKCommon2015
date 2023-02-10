using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace PKCommon
{
    public class PKUserUtils
    {
        private string strConnectionString = string.Empty;// ConfigurationManager.ConnectionStrings["PKRetailConnectionString"].ToString();

        public PKUserUtils(string connectionString)
        {
            strConnectionString = connectionString;
        }

        public List<TextValuePair> GetUserList()
        {
            SqlConnection cn = new SqlConnection();
            List<TextValuePair> userList = new List<TextValuePair>();
            try
            {
                string strsql = "SELECT UserID,UserName from PKUsers";
                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = strsql;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TextValuePair user = new TextValuePair();
                        user.MyText = dr["UserName"].ToString();
                        user.MyValue = dr["UserID"].ToString();
                        userList.Add(user);
                    }
                }
                cn.Close();
                return userList;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        
        }


        public List<TextValuePair> GetUserEmployeeList()
        {
            SqlConnection cn = new SqlConnection();
            List<TextValuePair> userList = new List<TextValuePair>();
            try
            {
                //string strsql = "SELECT UserID,UserName from PKUsers";
                string strsql = "select u.UserID, e.firstname + ' ' + e.lastname as UserName from pkemployee as e join pkusers as u on e.id=u.employeeid where e.status = 'Active' ";
                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = strsql;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TextValuePair user = new TextValuePair();
                        user.MyText = dr["UserName"].ToString();
                        user.MyValue = dr["UserID"].ToString();
                        userList.Add(user);
                    }
                }
                cn.Close();
                return userList;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

        }


        public string GetUserName(string userID)
        {
            SqlConnection cn = new SqlConnection();
            string userName = string.Empty;
            try
            {
                string strsql = @"SELECT UserName from PKUsers WHERE UserID=@UserID";
                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandText = strsql;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        userName = dr["UserName"].ToString();
                    }
                }
                cn.Close();
                return userName;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public string GetPassword(string userID)
        {
            SqlConnection cn = new SqlConnection();
            string password = string.Empty;
            try
            {
                string strsql = @"SELECT Password from PKUsers WHERE UserID=@UserID";
                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandText = strsql;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        password = dr["Password"].ToString();
                    }
                }
                cn.Close();
                return password;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public bool SavePassword(string userID, string password)
        {
            SqlConnection cn = new SqlConnection();
            string userName = string.Empty;
            string strNow = DateTime.Now.ToString(PKCommon.GlobalConst.strTimeStampFormat);
            try
            {
                string strsql = @"update PKUsers set Password=@Password, UpdateTime=@UpdateTime  WHERE UserID=@UserID";
                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@UpdateTime", strNow);
                cmd.CommandText = strsql;
                cn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }


        public static string SHA1ReadingFrameShift(string strCode)
        {
            string shiftedCode = string.Empty;
            string originalCode = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(strCode, System.Web.Configuration.FormsAuthPasswordFormat.SHA1.ToString());
            shiftedCode = originalCode.Substring(6, originalCode.Length - 6) + originalCode.Substring(0, 6);
            return shiftedCode;
        }


        public bool ClockIn(string strUserID, out string strClockTime, out string strError)
        {
            return ClockInOut(strUserID, ClockInOutType.ClockIn, out strClockTime, out strError);
        }

        public bool ClockOut(string strUserID, out string strClockTime, out string strError)
        {
            return ClockInOut(strUserID, ClockInOutType.ClockOut, out strClockTime, out strError);
        }

        private bool ClockInOut(string strUserID, ClockInOutType c, out string strClockTime, out string strError)
        {
            SqlConnection cn = new SqlConnection();
            strError = string.Empty;
            strClockTime = DateTime.Now.ToString(PKCommon.GlobalConst.strTimeStampFormat);
            try
            {
                string strsql = @"insert into PKClockInOut (ID, UserID,Type,ClockDateTime) values (@ID, @UserID,@Type,@ClockDateTime)";
                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@UserID", strUserID);
                cmd.Parameters.AddWithValue("@Type", (int)c);
                cmd.Parameters.AddWithValue("@ClockDateTime", strClockTime);
                cmd.CommandText = strsql;
                cn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return false;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

    }


    public enum ClockInOutType : int
    {
        ClockIn = 0,
        ClockOut = 1
    }
}