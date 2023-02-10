using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace PKCommon
{
    public class CustomerDiscountPolicyUtils
    {
        private string strConnectionString = string.Empty; 
            //ConfigurationManager.ConnectionStrings["RomeStation.Properties.Settings.PKRomeStnConnectionString"].ToString();\
        public CustomerDiscountPolicyUtils(string connectString)
        {
            strConnectionString = connectString;
        }

        public bool SaveCustomerDiscountPolicy(CustomerDiscountPolicyInfo newCustomerDiscountPolicy)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                string strSQL = @"INSERT INTO CustomerDiscountPolicy                              
                                    (ID, TotalPurchaseAmount, DiscountPercentage, CreateTime, UpdateTime, Status) 
                                    VALUES 
                                    (@ID, @TotalPurchaseAmount, @DiscountPercentage, @CreateTime, @UpdateTime, @Status)";

                cmd.Connection = cn;
                cmd.CommandText = strSQL;
                AddCmdParameters(newCustomerDiscountPolicy, cmd);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Save CustomerDiscountPolicy Error! Please contact your system provider.";
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

        private void AddCmdParameters(CustomerDiscountPolicyInfo CustomerDiscountPolicy, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = CustomerDiscountPolicy.ID;
            cmd.Parameters.Add("@TotalPurchaseAmount", SqlDbType.Decimal).Value = CustomerDiscountPolicy.TotalPurchaseAmount;
            cmd.Parameters.Add("@DiscountPercentage", SqlDbType.Decimal).Value = CustomerDiscountPolicy.DiscountPercentage;
            cmd.Parameters.Add("@CreateTime", SqlDbType.NVarChar, 50).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            cmd.Parameters.Add("@UpdateTime", SqlDbType.NVarChar, 50).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = CustomerDiscountPolicy.Status;
        }

        public bool UpdateCustomerDiscountPolicy(CustomerDiscountPolicyInfo CustomerDiscountPolicy)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update CustomerDiscountPolicy
                                    SET ID=@ID, TotalPurchaseAmount=@TotalPurchaseAmount,DiscountPercentage=@DiscountPercentage, Status=@Status, UpdateTime=@UpdateTime                                                                     
                                    WHERE ID=@ID";
                cmd.Connection = cn;
                AddCmdParameters(CustomerDiscountPolicy, cmd);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update CustomerDiscountPolicy Error! Please contact your system provider.";
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

        public bool DeleteCustomerDiscountPolicy(string CustomerDiscountPolicyID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Update CustomerDiscountPolicy
                                    SET Status=@Status                           
                                    WHERE ID=@ID";
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", CustomerDiscountPolicyID);
                cmd.Parameters.AddWithValue("@Status", Global.DiscountPolicy_STATUS_Deleted);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Update CustomerDiscountPolicy Error! Please contact your system provider.";
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

        public CustomerDiscountPolicyInfo GetDiscountPolicyInfo(string strDiscountPolicyID)
        {
            SqlConnection cn = new SqlConnection();
            CustomerDiscountPolicyInfo cdpi = new CustomerDiscountPolicyInfo();
            try
            {
                string strSQL = @"SELECT ID, TotalPurchaseAmount, DiscountPercentage, CreateTime, UpdateTime, Status
                                  FROM CustomerDiscountPolicy
                                  WHERE ID=@ID";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ID", strDiscountPolicyID);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cdpi.ID = dr["ID"].ToString();
                        if (dr["TotalPurchaseAmount"] != null && dr["TotalPurchaseAmount"].ToString() != string.Empty)
                        {
                            cdpi.TotalPurchaseAmount = (decimal)dr["TotalPurchaseAmount"];
                        }
                        else
                        {
                            cdpi.TotalPurchaseAmount = 0.00m;
                        }

                        if (dr["DiscountPercentage"] != null && dr["DiscountPercentage"].ToString() != string.Empty)
                        {
                            cdpi.DiscountPercentage = (decimal)dr["DiscountPercentage"];
                        }
                        else
                        {
                            cdpi.DiscountPercentage = 0.00m;
                        }
                        if (dr["CreateTime"] != null && dr["CreateTime"].ToString() != null && dr["CreateTime"].ToString() != string.Empty && dr["CreateTime"].ToString() != "")
                        {
                            cdpi.CreateTime = ((DateTime)dr["CreateTime"]).ToShortDateString();
                        }
                        else
                        {
                            cdpi.CreateTime = "";
                        }

                        if (dr["UpdateTime"] != null && dr["UpdateTime"].ToString() != null && dr["UpdateTime"].ToString() != string.Empty && dr["UpdateTime"].ToString() != "")
                        {
                            cdpi.UpdateTime = ((DateTime)dr["UpdateTime"]).ToShortDateString();
                        }
                        else
                        {
                            cdpi.UpdateTime = "";
                        }
                        cdpi.Status = dr["Status"].ToString();
                    }
                }
                cn.Close();
                return cdpi;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get GetDiscountPolicy List Error! Please contact your system provider.";
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

        public List<CustomerDiscountPolicyInfo> GetDiscountPolicyList()
        {
            SqlConnection cn = new SqlConnection();
            List<CustomerDiscountPolicyInfo> cdpiList = new List<CustomerDiscountPolicyInfo>();

            try
            {
                string strSQL = @"SELECT ID, TotalPurchaseAmount, DiscountPercentage, CreateTime, UpdateTime, Status
                                  FROM CustomerDiscountPolicy
                                  WHERE Status!=@Status
                                  ORDER BY TotalPurchaseAmount DESC";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@Status", Global.DiscountPolicy_STATUS_Deleted);
                cmd.CommandText = strSQL;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        CustomerDiscountPolicyInfo cdpi = new CustomerDiscountPolicyInfo();
                        cdpi.ID = dr["ID"].ToString();
                        if (dr["TotalPurchaseAmount"] != null && dr["TotalPurchaseAmount"].ToString() != string.Empty)
                        {
                            cdpi.TotalPurchaseAmount = (decimal)dr["TotalPurchaseAmount"];
                        }
                        else
                        {
                            cdpi.TotalPurchaseAmount = 0.00m;
                        }

                        if (dr["DiscountPercentage"] != null && dr["DiscountPercentage"].ToString() != string.Empty)
                        {
                            cdpi.DiscountPercentage = (decimal)dr["DiscountPercentage"];
                        }
                        else
                        {
                            cdpi.DiscountPercentage = 0.00m;
                        }

                        if (dr["CreateTime"] != null && dr["CreateTime"].ToString() != null && dr["CreateTime"].ToString() != string.Empty && dr["CreateTime"].ToString() != "")
                        {
                            cdpi.CreateTime = ((DateTime)dr["CreateTime"]).ToShortDateString();
                        }
                        else
                        {
                            cdpi.CreateTime = "";
                        }

                        if (dr["UpdateTime"] != null && dr["UpdateTime"].ToString() != null && dr["UpdateTime"].ToString() != string.Empty && dr["UpdateTime"].ToString() != "")
                        {
                            cdpi.UpdateTime = ((DateTime)dr["UpdateTime"]).ToShortDateString();
                        }
                        else
                        {
                            cdpi.UpdateTime = "";
                        }
                        cdpi.Status = dr["Status"].ToString();

                        cdpiList.Add(cdpi);
                    }
                }
                cn.Close();
                return cdpiList;
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                Global.DATABASE_ACCESS_ERROR_MESSAGE = "Database Get GetDiscountPolicy List Error! Please contact your system provider.";
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

    }

}

