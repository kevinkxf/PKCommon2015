using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;

namespace PKCommon
{
    public class PKPackingQtyMappingUtils
    {
        public static List<PKPackingQtyMappingInfo> GetPackingQtyMappingList(string strConnectionString, string productID)
        {

            SqlConnection cn = new SqlConnection();
            List<PKPackingQtyMappingInfo> packingQtyMappingList = new List<PKPackingQtyMappingInfo>();

            try
            {
                Logger.AddToFile("GetPackingQtyMappingList->1");
                string strsql = "SELECT ProductID, BaseProductID, PackingQty, UnitID FROM PKPackingQtyMapping where (ProductID=@ProductID)";
                Logger.AddToFile(strsql);

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ProductID", productID);
                cmd.CommandText = strsql;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    //--Kevin  20131029//Logger.AddToFile("GetPackingQtyMappingList->2");
                    while (dr.Read())
                    {
                        //--Kevin  20131029//Logger.AddToFile("GetPackingQtyMappingList->13");
                        PKPackingQtyMappingInfo PKpqpi = new PKPackingQtyMappingInfo();
                        PKpqpi.ProductID = dr["ProductID"].ToString();
                        PKpqpi.BaseProductID = dr["BaseProductID"].ToString();
                        PKpqpi.PackingQty = (decimal)dr["PackingQty"];
                        PKpqpi.UnitID = (int)dr["UnitID"];
                        packingQtyMappingList.Add(PKpqpi);
                    }
                }
                cn.Close();
                return packingQtyMappingList;
            }
            catch (Exception)
            {
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

        public static bool SavePackingQtyMapping(string strConnectionString, PKPackingQtyMappingInfo newPKPackingQtyMappingInfo)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"insert into PKPackingQtyMapping                               
                                (ProductID, BaseProductID, PackingQty, UnitID)
                                values 
                                (@ProductID, @BaseProductID, @PackingQty, @UnitID)";
                cmd.Connection = cn;

                AddCmdParameters(newPKPackingQtyMappingInfo, cmd);

                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();

                return true;

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

        private static void AddCmdParameters(PKPackingQtyMappingInfo packingQtyMapping, SqlCommand cmd)
        {
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = packingQtyMapping.ID;
            cmd.Parameters.Add("@ProductID", SqlDbType.NVarChar, 50).Value = packingQtyMapping.ProductID;
            cmd.Parameters.Add("@BaseProductID", SqlDbType.NVarChar, 50).Value = packingQtyMapping.BaseProductID;
            cmd.Parameters.Add("@PackingQty", SqlDbType.Decimal).Value = packingQtyMapping.PackingQty;
            cmd.Parameters.Add("@UnitID", SqlDbType.Int).Value = packingQtyMapping.UnitID;
        }

        public static bool UpdatePackingQtyMapping(string strConnectionString, PKPackingQtyMappingInfo updatedPKPackingQtyMappingInfo)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"update PKPackingQtyMapping set                             
                                             ProductID=@ProductID, BaseProductID=@BaseProductID, PackingQty=@PackingQty, UnitID=@UnitID
                                             WHERE (ProductID=@ProductID) and (BaseProductID=@BaseProductID)";

                cmd.Connection = cn;
                AddCmdParameters(updatedPKPackingQtyMappingInfo, cmd);

                cn.Open();
                int intResult = cmd.ExecuteNonQuery();
                cn.Close();

                if (intResult > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //TODO log
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

        public static bool DeletePackingQtyMapping(string strConnectionString, string id)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = strConnectionString;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"DELETE PKPackingQtyMapping WHERE ID= " + id;
                cmd.Connection = cn;
                cn.Open();
                int intResult = cmd.ExecuteNonQuery();
                cn.Close();

                if (intResult > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //TODO log
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

        public static List<PKPackingQtyMappingInfo> GetPackingQtyMappingList(string strConnectionString, string productID, string baseProductID)
        {
            SqlConnection cn = new SqlConnection();
            List<PKPackingQtyMappingInfo> packingQtyMappingList = new List<PKPackingQtyMappingInfo>();

            try
            {
                string strsql = "SELECT ProductID, BaseProductID, PackingQty, UnitID FROM PKPackingQtyMapping where (ProductID=@ProductID) and (BaseProductID=@BaseProductID)";

                cn.ConnectionString = strConnectionString;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ProductID", productID);
                cmd.Parameters.AddWithValue("@BaseProductID", baseProductID);
                cmd.CommandText = strsql;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PKPackingQtyMappingInfo PKpqpi = new PKPackingQtyMappingInfo();
                        PKpqpi.ProductID = dr["ProductID"].ToString();
                        PKpqpi.BaseProductID = dr["BaseProductID"].ToString();
                        PKpqpi.PackingQty = (decimal)dr["PackingQty"];
                        PKpqpi.UnitID = (int)dr["UnitID"];
                        packingQtyMappingList.Add(PKpqpi);
                    }
                }
                cn.Close();
                return packingQtyMappingList;
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

    }
}
