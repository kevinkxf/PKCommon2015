using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace PKCommon
{
    public class TourUtil
    {
        private string strConnectionString = string.Empty;

        public TourUtil(string connectionstring)
        {
            strConnectionString = connectionstring;

        }
        public string GetAgentNameFromID(string ID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Select Name From TourAgent Where ID='" + ID + "'";
                cmd.Connection = cn;
                cn.Open();
                return cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                PKCommon.Logger.AddToFile(ex.Message);
                return "";
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }

        public string GetTourGuideNameFromID(string ID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Select FirstName+' '+LastName as Name From TourGuide Where ID='" + ID + "'";
                cmd.Connection = cn;
                cn.Open();
                return cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                PKCommon.Logger.AddToFile(ex.Message);
                return "";
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }
        public string GetNewTourCode(string companyCode, string date)
        {
            SqlConnection cn = new SqlConnection(strConnectionString);
            DataSet ds = new DataSet();
            string sTourCodePre = companyCode.Trim() + date.Replace("-", "").Substring(2);
            string sTourCode= "";
            string sqlStr = "SELECT TourCode FROM Tour WHERE EstArrDate >='" + date + " 00:00:00' AND EstArrDate <= '"+ date +"  23:59:59.999'";
            try
            {
                cn.Open();
                //  string sqlStr = "SELECT ID, CompanyName From Customer ";
                SqlCommand cmd = new SqlCommand(sqlStr, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    int lastNumber = 0;
                    int number = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sLastTourCode = dr[0].ToString();                       
                        try
                        {
                            number = Int32.Parse(sLastTourCode.Substring(sLastTourCode.Length - 3, 3));
                        }
                        catch
                        {
                            number=0;
                        }
                        if (number > lastNumber)
                            lastNumber = number;
                    }
                    ++lastNumber;
                    if (lastNumber < 10)
                        sTourCode = sTourCodePre + "00" + lastNumber.ToString();
                    else if (lastNumber < 100)
                        sTourCode = sTourCodePre + "0" + lastNumber.ToString();
                    else
                        sTourCode =sTourCodePre + lastNumber.ToString();
                }
                else
                {
                    sTourCode =sTourCodePre + "001";
                }
                bool hasTourCode = true;
                while (hasTourCode)
                {
                     cmd.CommandText = "SELECT TourCode FROM Tour WHERE TourCode ='" + sTourCode + "'";
                     if (cmd.ExecuteScalar() == null)
                         hasTourCode = false;
                     else
                     {
                         try
                         {
                            int number = Int32.Parse(sTourCode.Substring(sTourCode.Length - 3, 3))+1;
                            if (number < 10)
                                sTourCode = sTourCodePre + "00" + number.ToString();
                            else if (number < 100)
                                sTourCode = sTourCodePre + "0" + number.ToString();
                            else
                                sTourCode = sTourCodePre + number.ToString();
                         }
                         catch
                         {
                             return sTourCodePre+"000";
                         }
                     }
                }
                return sTourCode;
                
            }
            catch (Exception ex)
            {
                throw (new Exception("Error in GetDataTable", ex));
            }
            finally
            {
                if (cn != null) cn.Close();
                if (ds != null) ds.Dispose();
            }

        }



        private string strTempTourCode = "SP";
        public bool SaveNewTempTour(string CreateBy, string UpdateBy, out string strTourCode, out string strError)
        {
            strTourCode = GetNewTourCode(strTempTourCode, DateTime.Now.ToString(PKCommon.GlobalConst.strDateStampFormat));

            strError = string.Empty;

            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"insert into Tour  (Date,TourCode,CreateDate,CreateBy,Updatedate,Updateby,Status) values (@Date,@TourCode,@CreateDate,@CreateBy,@Updatedate,@Updateby,@Status); ";
                cmd.Connection = cn;
                cmd.Parameters.Add("@Date", SqlDbType.NVarChar, 100).Value = DateTime.Now.ToString(PKCommon.GlobalConst.strDateStampFormat);
                cmd.Parameters.Add("@TourCode", SqlDbType.NVarChar, 50).Value = strTourCode;
                cmd.Parameters.Add("@CreateDate", SqlDbType.NVarChar, 50).Value = DateTime.Now.ToString(PKCommon.GlobalConst.strTimeStampFormat);
                cmd.Parameters.Add("@CreateBy", SqlDbType.NVarChar, 50).Value = CreateBy;
                cmd.Parameters.Add("@Updatedate", SqlDbType.NVarChar, 50).Value = DateTime.Now.ToString(PKCommon.GlobalConst.strTimeStampFormat);
                cmd.Parameters.Add("@UpdateBy", SqlDbType.NVarChar, 50).Value = UpdateBy;
                cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = "Pending";
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
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
}
/*
 public string GetNewTourCode(string companyCode, string date)
        {
            SqlConnection cn = new SqlConnection(strConnectionString);
            DataSet ds = new DataSet();
            string sTourCode = companyCode.Trim() + date.Replace("-", "").Substring(2);
            string sqlStr = "SELECT Top 1 TourCode FROM Tour WHERE Date ='" + date + "' ORDER BY CreateDate DESC";
            try
            {
                cn.Open();
                //  string sqlStr = "SELECT ID, CompanyName From Customer ";
                SqlCommand cmd = new SqlCommand(sqlStr, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string sLastTourCode = dt.Rows[0][0].ToString();
                    int number = Convert.ToInt32(sLastTourCode.Substring(sLastTourCode.Length - 3, 3)) + 1;
                    if (number < 10)
                        sTourCode += "00" + number.ToString();
                    else if (number < 100)
                        sTourCode += "0" + number.ToString();
                    else
                        sTourCode += number.ToString();
                }
                else
                {
                    sTourCode += "001";
                }
                return sTourCode;
            }
            catch (Exception ex)
            {
                throw (new Exception("Error in GetDataTable", ex));
            }
            finally
            {
                if (cn != null) cn.Close();
                if (ds != null) ds.Dispose();
            }

        }
*/