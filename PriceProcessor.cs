using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace PKCommon
{
    public class PriceProcessor
    {
        private string connectionString;
        //public string ConnectionString
        //{
        //    get { return connectionString; }
        //    set { connectionString = value; }
        //}

        private DataTable priceDataTable;
        //public DataTable PriceDataTable
        //{
        //    get { return priceDataTable; }
        //    set { priceDataTable = value; }
        //}

        private bool isServer;
        //public bool IsServer
        //{
        //    get { return isServer; }
        //    set { isServer = value; }
        //}

        private string storeID;
        //public string StoreID
        //{
        //    get { return storeID; }
        //    set { storeID = value; }
        //}

        public PriceProcessor(string strConnectionString, DataTable dtPrice, bool serverFlag, string strStoreID)
        {
            this.connectionString = strConnectionString;
            this.priceDataTable = dtPrice;
            this.isServer = serverFlag;
            this.storeID = strStoreID;
        }

        /// <summary>
        /// update all price data to temp price table on the server
        /// </summary>
        /// <returns></returns>
        public bool SynchronizeAllPriceData2TempPriceTable()
        {
            SqlConnection cn = new SqlConnection();
            try
            {
                //delete temp price data
                cn.ConnectionString = connectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "delete from pricetemp";
                    cmd.Connection = cn;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                };
                //copy all the price data to the temp table
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    sqlbulkcopy.DestinationTableName = "pricetemp";
                    sqlbulkcopy.WriteToServer(priceDataTable);
                    sqlbulkcopy.Close();
                };
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in SynchronizeAllPriceData2TempPriceTable:" + ex.Message, storeID);
                return false;
            }
            finally
            {
                cn.Close();
            }
        }

        /// <summary>
        /// Process local price/PKPrice data by the temp price data
        /// </summary>
        public void ProcessPriceData()
        {
            try
            {
                Logger.AddToFile("DeleteRedundantpriceData", storeID);
                DeleteRedundantPriceData();

                Logger.AddToFile("Update/Insert PriceData", storeID);
                UpdatePriceData();

                if (isServer)
                {
                    Logger.AddToFile("Update/Insert PKPrice Data ==================", storeID);
                    UpdatePKPriceData();

                    Logger.AddToFile("Update/Insert PKProductTax Data ====================");
                    UpdatePKProductTaxData();
                }
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in ProcessPriceData: =====================", storeID);
                Logger.AddToFile("Error: " + ex.Message, storeID);
            }
        }

        private void DeleteRedundantPriceData()
        {
            SqlDataReader drTarget = null;
            SqlConnection cnnSource = new SqlConnection(connectionString);
            SqlConnection cnnTarget = new SqlConnection(connectionString);
            try
            {
                if (cnnSource.State == ConnectionState.Closed)
                {
                    cnnSource.Open();
                }
                if (cnnTarget.State == ConnectionState.Closed)
                {
                    cnnTarget.Open();
                }

                SqlConnection cnnTargetDelete = new SqlConnection(connectionString);

                SqlCommand cmdTargetSelect = new SqlCommand();
                cmdTargetSelect.Connection = cnnTarget;
                cmdTargetSelect.CommandText = "SELECT [uid],[productname],[pricetype],[priceqty] FROM price";

                drTarget = cmdTargetSelect.ExecuteReader();
                int updatedRowCount = 0;
                while (drTarget.Read())
                {
                    string struid = drTarget["uid"].ToString();
                    string strProductName = drTarget["productname"].ToString();
                    string strPriceType = drTarget["pricetype"].ToString();
                    string strPriceQty = drTarget["priceqty"].ToString();

                    SqlCommand cmdSourceSelect = new SqlCommand();
                    cmdSourceSelect.Connection = cnnSource;
                    cmdSourceSelect.CommandText = "select uid from pricetemp where  productname=@productname and pricetype=@pricetype and priceqty=@priceqty";
                    cmdSourceSelect.Parameters.AddWithValue("@productname", strProductName);
                    cmdSourceSelect.Parameters.AddWithValue("@pricetype", strPriceType);
                    cmdSourceSelect.Parameters.AddWithValue("@priceqty", strPriceQty);

                    using (SqlDataReader drSource = cmdSourceSelect.ExecuteReader())
                    {
                        if (!drSource.HasRows)
                        {
                            SqlCommand cmdTargetDelete = new SqlCommand();
                            cmdTargetDelete.Connection = cnnTargetDelete;
                            cmdTargetDelete.CommandText = "delete from price where uid='" + struid + "'";

                            cnnTargetDelete.Open();
                            cmdTargetDelete.ExecuteNonQuery();
                            cnnTargetDelete.Close();
                            updatedRowCount++;
                        }
                    };

                }
                cnnSource.Close();
                cnnTarget.Close();
                Logger.AddToFile(updatedRowCount.ToString() + " rows price data have been removed.", storeID);
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in DeleteRedundantProductData:" + ex.Message, storeID);
            }
            finally
            {
                cnnSource.Close();
                cnnTarget.Close();
            }
        }

        /// <summary>
        /// update price table
        /// </summary>
        private void UpdatePriceData()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = connectionString;

            SqlConnection pricecn = new SqlConnection();
            pricecn.ConnectionString = connectionString;
            pricecn.Open();
            //Logger.AddToFile("pricecn.beginTransaction");
            //trans = pricecn.BeginTransaction();
            bool isUpdate = false;//identfy the opertion: true->update, false->insert
            int intErrorCounter = 0;
            int intInstertedCounter = 0;
            int intUpdatedCounter = 0;
            int intSkipOverwriteCounter = 0;

            string strUid = string.Empty;
            string strProductName = string.Empty;
            string strPriceType = string.Empty;
            string strPriceQyt = string.Empty;
            try
            {
                string strUpdateCommandText = @"update price set ";
                //productname=@productname and  pricetype=@pricetype
                strUpdateCommandText += " storeid=@storeid, ";
                strUpdateCommandText += " priceqty=@priceqty, ";
                strUpdateCommandText += " pricevalue=@pricevalue, ";
                strUpdateCommandText += " priceeffectivetime=@priceeffectivetime, ";
                strUpdateCommandText += " status=@status, ";
                strUpdateCommandText += " username=@username, ";
                strUpdateCommandText += " pricecreatedtime=@pricecreatedtime ";
                strUpdateCommandText += " WHERE uid=@uid";

                string strInsertFields = string.Empty;
                string strInsertValues = string.Empty;
                strInsertFields += "productname, ";
                strInsertValues += "@productname, ";
                strInsertFields += " storeid, ";
                strInsertValues += " @storeid, ";
                strInsertFields += " pricetype, ";
                strInsertValues += " @pricetype, ";
                strInsertFields += " priceqty, ";
                strInsertValues += " @priceqty, ";
                strInsertFields += " pricevalue, ";
                strInsertValues += " @pricevalue, ";
                strInsertFields += " priceeffectivetime, ";
                strInsertValues += " @priceeffectivetime, ";
                strInsertFields += " status, ";
                strInsertValues += " @status, ";
                strInsertFields += " username, ";
                strInsertValues += " @username, ";
                strInsertFields += " pricecreatedtime";
                strInsertValues += " @pricecreatedtime";

                string strInsertCommandText = "INSERT INTO price (" + strInsertFields + ") VALUES (" + strInsertValues + ")";

                for (int i = 0; i < priceDataTable.Rows.Count; i++)
                {
                    //Logger.AddToFile("i=" + i);
                    //identify if price exists
                    strProductName = priceDataTable.Rows[i]["productname"].ToString();
                    strPriceType = priceDataTable.Rows[i]["pricetype"].ToString();
                    strPriceQyt = priceDataTable.Rows[i]["priceqty"].ToString();

                    isUpdate = false;
                    strUid = string.Empty;

                    //Logger.AddToFile("productName=" + strProductName);
                    using (SqlCommand cmdSelect = new SqlCommand())
                    {
                        cmdSelect.Connection = cn;
                        bool ispriceoverwritable = true;
                        if (isServer)
                        {
                            cmdSelect.CommandText = "select uid from price where productname=@productname and pricetype=@pricetype and priceqty=@priceqty";
                        }
                        else
                        {
                            //get priceoverwrite on client side
                            cmdSelect.CommandText = "select price.[uid], product.[priceoverwrite] from price left join product on price.[productname]=product.[productname] where price.[productname]=@productname and pricetype=@pricetype and priceqty=@priceqty";
                        }
                        cmdSelect.Parameters.AddWithValue("@productname", strProductName);
                        cmdSelect.Parameters.AddWithValue("@pricetype", strPriceType);
                        cmdSelect.Parameters.AddWithValue("@priceqty", strPriceQyt);
                        //Logger.AddToFile("To open cn");
                        cn.Open();
                        //Logger.AddToFile("To execute reader");
                        SqlDataReader dr = cmdSelect.ExecuteReader();
                        while (dr.Read())
                        {
                            //check priceoverwrite on client side                        
                            if (!isServer)
                            {
                                if (string.Equals(dr["priceoverwrite"].ToString(), "no", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    ispriceoverwritable = false;
                                }
                            }
                            isUpdate = true;
                            strUid = dr["uid"].ToString();
                        }
                        //Logger.AddToFile("isupdat = " + isUpdate);
                        cn.Close();
                        //Logger.AddToFile("cn closed");
                        if (!ispriceoverwritable)
                        {
                            intSkipOverwriteCounter++;
                            //Logger.AddToFile("Skipped server price for productname=" + strProductName + ", kept local price");
                            continue;
                        }
                    };
                    using (SqlCommand cmdExecute = new SqlCommand())
                    {
                        //uid, productname, storeid, pricetype, priceqty, pricevalue, priceeffectivetime, status, username, pricecreatedtime
                        cmdExecute.Connection = pricecn;

                        if (isUpdate)
                        {
                            intUpdatedCounter++;
                            cmdExecute.CommandText = strUpdateCommandText;
                            cmdExecute.Parameters.AddWithValue("@uid", strUid);
                        }
                        else
                        {
                            intInstertedCounter++;
                            cmdExecute.CommandText = strInsertCommandText;
                        }

                        cmdExecute.Parameters.AddWithValue("@productname", priceDataTable.Rows[i]["productname"]);
                        cmdExecute.Parameters.AddWithValue("@storeid", storeID);
                        cmdExecute.Parameters.AddWithValue("@pricetype", priceDataTable.Rows[i]["pricetype"]);
                        cmdExecute.Parameters.AddWithValue("@priceqty", priceDataTable.Rows[i]["priceqty"]);
                        cmdExecute.Parameters.AddWithValue("@pricevalue", priceDataTable.Rows[i]["pricevalue"]);
                        cmdExecute.Parameters.Add("@priceeffectivetime", SqlDbType.DateTime).Value = priceDataTable.Rows[i]["priceeffectivetime"];
                        //cmdExecute.Parameters.AddWithValue("@priceeffectivetime", priceDataTable.Rows[i]["priceeffectivetime"]);
                        cmdExecute.Parameters.AddWithValue("@status", priceDataTable.Rows[i]["status"]);
                        cmdExecute.Parameters.AddWithValue("@username", priceDataTable.Rows[i]["username"]);
                        cmdExecute.Parameters.Add("@pricecreatedtime", SqlDbType.DateTime).Value = priceDataTable.Rows[i]["pricecreatedtime"];
                        //cmdExecute.Parameters.AddWithValue("@pricecreatedtime", priceDataTable.Rows[i]["pricecreatedtime"]);

                        //Logger.AddToFile("price parameter added");
                        int affectedRows = cmdExecute.ExecuteNonQuery();
                        //Logger.AddToFile("affectedrows=" + affectedRows);
                        if (affectedRows <= 0)
                        {
                            intErrorCounter++;
                        }
                    };
                }
                pricecn.Close();
            }
            catch (Exception ex)
            {
                Logger.AddToFile(string.Format("Error in UpdatePriceData: {0}", ex.Message), storeID);
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
                if (pricecn.State == ConnectionState.Open)
                {
                    pricecn.Close();
                }
                Logger.AddToFile(string.Format("The intErrorCounter={0}", intErrorCounter), storeID);
                Logger.AddToFile(string.Format("The intInstertedCounter={0}", intInstertedCounter), storeID);
                Logger.AddToFile(string.Format("The intUpdatedCounter={0}", intUpdatedCounter), storeID);
                Logger.AddToFile(string.Format("The intSkipOverwriteCounter={0}", intSkipOverwriteCounter), storeID);
            }
        }

        private void UpdatePKPriceData()
        {
            string commandText = string.Empty; ;
            //string strStream = string.Empty;
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection();
            try
            {
                commandText = "SELECT uid, productname, storeid, pricetype, priceqty, pricevalue, priceeffectivetime, status, username, pricecreatedtime FROM pricetemp WHERE (pricetype = 'price') AND (priceqty = 1) ";
                //strStream = PKCommon.CommonFunctions.Read2Stream(connectionString, commandText, out strErr);
                cn.ConnectionString = connectionString;
                SqlDataAdapter da = new SqlDataAdapter(commandText, cn);
                cn.Open();
                da.Fill(dt);
                cn.Close();

                Logger.AddToFile("Process PKPrice Data", storeID);
                if (TransferPOSPriceData(dt))
                {
                    Logger.AddToFile("Process PKPrice Data success", storeID);
                }
                else
                {
                    Logger.AddToFile("Process PKPrice Data failed", storeID);
                }

            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in UpdatePKPriceData:" + ex.Message, storeID);
            }
            finally
            {
                cn.Close();
            }
        }

        private bool TransferPOSPriceData(DataTable dtPrice)
        {
            bool isUpdated = false;
            int intCannotFindProductIDCounter = 0;
            int intUpdateCounter = 0;
            int intInsertCounter = 0;
            int intErrorCounter = 0;

            SqlConnection sqlCnn = new SqlConnection();
            try
            {
                //SELECT ID, ProductID, Cost, A, B, C, D, E, Special, CreateTime, UpdateTime, Creater, Updater
                string strID = string.Empty;
                string strProductName = string.Empty;
                string strProductID = string.Empty;
                string strCost = string.Empty;
                string strA = string.Empty;
                string strB = string.Empty;
                string strC = string.Empty;
                string strD = string.Empty;
                string strE = string.Empty;
                string strSpecial = string.Empty;
                string strCreateTime = string.Empty;
                string strUpdateTime = string.Empty;
                string strCreater = string.Empty;
                string strUpdater = string.Empty;

                Logger.AddToFile("TransferPOSPriceData", storeID);

                if (dtPrice.Rows.Count <= 0)
                {
                    Logger.AddToFile("No data for POS Price.", storeID);
                    return false;
                }

                string strUpdateCommandText = "UPDATE PKPrice SET ProductID=@ProductID, Cost=@Cost, A=@A, B=@B, C=@C, D=@D, E=@E, Special=@Special, CreateTime=@CreateTime, UpdateTime=@UpdateTime, Creater=@Creater, Updater=@Updater WHERE ID=@ID";
                string strInsertCommandText = "insert into PKPrice (ID, ProductID, Cost, A, B, C, D, E, Special, CreateTime, UpdateTime, Creater, Updater) values (@ID, @ProductID, @Cost, @A, @B, @C, @D, @E, @Special, @CreateTime, @UpdateTime, @Creater, @Updater)";

                //Logger.AddToFile("Setup database connection for POS Price data update!", storeID);
                sqlCnn.ConnectionString = connectionString;
                sqlCnn.Open();

                for (int rowIndex = 0; rowIndex < dtPrice.Rows.Count; rowIndex++)
                {
                    //Logger.AddToFile("i=" + rowIndex);

                    //uid, productname, storeid, pricetype, priceqty, pricevalue, priceeffectivetime, status, username, pricecreatedtime 
                    //strID = dtPrice.Rows[rowIndex]["plu"].ToString();
                    strProductName = dtPrice.Rows[rowIndex]["productname"].ToString();
                    //strProductID = dtPrice.Rows[rowIndex]["plu"].ToString();
                    //strCost = "0";
                    strCost = dtPrice.Rows[rowIndex]["AverageCost"].ToString();
                    strA = dtPrice.Rows[rowIndex]["pricevalue"].ToString();
                    strB = "0";// dtPrice.Rows[rowIndex]["pricevalue"].ToString();
                    strC = "0";//dtPrice.Rows[rowIndex]["pricevalue"].ToString();
                    strD = "0";//dtPrice.Rows[rowIndex]["pricevalue"].ToString();
                    strE = "0";//dtPrice.Rows[rowIndex]["pricevalue"].ToString();
                    strSpecial = "0";//dtPrice.Rows[rowIndex]["pricevalue"].ToString();
                    DateTime tempDT;
                    DateTime.TryParse(dtPrice.Rows[rowIndex]["pricecreatedtime"].ToString(), out tempDT);
                    strCreateTime = tempDT.ToString(GlobalConst.strTimeStampFormat);
                    strUpdateTime = tempDT.ToString(GlobalConst.strTimeStampFormat);
                    strCreater = "Imported";
                    strUpdater = "Imported";
                    using (SqlCommand cmdProductID = new SqlCommand())
                    {
                        //cmdProductID.CommandText = "SELECT ID FROM PKProduct WHERE (Name1 = '" + strProductName + "')";
                        cmdProductID.CommandText = "SELECT ID FROM PKProduct WHERE (Name1 = @Name1)";
                        cmdProductID.Parameters.AddWithValue("@Name1", strProductName);
                        cmdProductID.Connection = sqlCnn;
                        SqlDataReader drProductID = cmdProductID.ExecuteReader();
                        while (drProductID.Read())
                        {
                            strProductID = drProductID[0].ToString();
                            break;
                        }
                        drProductID.Close();
                    }
                    if (strProductID.Length == 0)
                    {
                        intCannotFindProductIDCounter++;
                        //Logger.AddToFile("Can not find the productID for " + strProductName + ", Skip the price update.");
                        continue;
                    }

                    //Logger.AddToFile("Setup parameters for POS Price data update.");

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlCnn;

                    bool isUpdateFlag = false;
                    //check exist
                    using (SqlCommand cmdDistinct = new SqlCommand())
                    {
                        cmdDistinct.Connection = sqlCnn;
                        cmdDistinct.CommandText = "SELECT ID FROM PKPrice WHERE (ProductID = @ProductID)";
                        cmdDistinct.Parameters.AddWithValue("@ProductID", strProductID);
                        using (SqlDataReader dr = cmdDistinct.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                strID = dr["ID"].ToString();
                                isUpdateFlag = true;
                                //Logger.AddToFile("Found PKPrice to updat, ID=" + strID);
                            }
                        }
                        //cmdDistinct.Dispose();
                        //dr.Close();
                    };

                    if (isUpdateFlag)
                    {
                        intUpdateCounter++;
                        //Logger.AddToFile("Update PKPrice, ProductName=" + strProductName);
                        cmd.CommandText = strUpdateCommandText;
                    }
                    else
                    {
                        intInsertCounter++;
                        //Logger.AddToFile("Insert PKPrice, ProductName=" + strProductName);
                        cmd.CommandText = strInsertCommandText;
                        strID = Guid.NewGuid().ToString();
                    }

                    //@ID, @ProductID, @Cost, @A, @B, @C, @D, @E, @Special, @CreateTime, @UpdateTime, @Creater, @Updater
                    cmd.Parameters.AddWithValue("@ID", strID);
                    cmd.Parameters.AddWithValue("@ProductID", strProductID);
                    cmd.Parameters.AddWithValue("@Cost", strCost);
                    cmd.Parameters.AddWithValue("@A", strA);
                    cmd.Parameters.AddWithValue("@B", strB);
                    cmd.Parameters.AddWithValue("@C", strC);
                    cmd.Parameters.AddWithValue("@D", strD);
                    cmd.Parameters.AddWithValue("@E", strE);
                    cmd.Parameters.AddWithValue("@Special", strSpecial);
                    cmd.Parameters.AddWithValue("@CreateTime", strCreateTime);
                    cmd.Parameters.AddWithValue("@UpdateTime", strUpdateTime);
                    cmd.Parameters.AddWithValue("@Creater", strCreater);
                    cmd.Parameters.AddWithValue("@Updater", strUpdater);

                    //Logger.AddToFile("Begin execute.");
                    int intRows = cmd.ExecuteNonQuery();
                    if (intRows > 0)
                    {
                        isUpdated = true;
                        //Logger.AddToFile("Updated PKPrice, ProductName=" + strProductName);
                    }
                    else
                    {
                        intErrorCounter++;
                        isUpdated = false;
                        //Logger.AddToFile("Update PKPrice failed, ProductName=" + strProductName);
                    }
                }
                sqlCnn.Close();
                return isUpdated;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in TransferPOSPriceData:" + ex.Message, storeID);
                isUpdated = false;
                return isUpdated;
            }
            finally
            {
                if (sqlCnn.State == ConnectionState.Open)
                {
                    sqlCnn.Close();
                }
                Logger.AddToFile(string.Format("The intErrorCounter={0}", intErrorCounter), storeID);
                Logger.AddToFile(string.Format("The intCannotFindProductIDCounter={0}", intCannotFindProductIDCounter), storeID);
                Logger.AddToFile(string.Format("The intInsertCounter={0}", intInsertCounter), storeID);
                Logger.AddToFile(string.Format("The intUpdateCounter={0}", intUpdateCounter), storeID);
            }
        }

        private void UpdatePKProductTaxData()
        {
            string commandText = string.Empty; ;
            //string strStream = string.Empty;
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection();
            try
            {
                //gst - HST 12%
                //commandText = "SELECT uid, productname, storeid, pricetype, priceqty, pricevalue, priceeffectivetime, status, username, pricecreatedtime FROM pricetemp WHERE (pricetype = 'gst') AND (priceqty = 1) AND (status = 'available')";
                commandText = "SELECT productname FROM pricetemp WHERE (pricetype = 'gst') AND (priceqty = 1) AND (status = 'available')";
                cn.ConnectionString = connectionString;
                SqlDataAdapter dagst = new SqlDataAdapter(commandText, cn);
                cn.Open();
                dagst.Fill(dt);
                cn.Close();

                Logger.AddToFile("Process PKProductTax Data", storeID);
                if (TransferPOSTaxData(dt, "TAX100", true))
                {
                    Logger.AddToFile("Process gst to PKProductTax Data success", storeID);
                }
                else
                {
                    Logger.AddToFile("Process gst to PKProductTax Data failed", storeID);
                }
                //pst - HST 5%
                dt.Clear();
                commandText = "SELECT productname FROM pricetemp WHERE (pricetype = 'pst') AND (priceqty = 1) AND (status = 'available')";
                cn.ConnectionString = connectionString;
                SqlDataAdapter dapst = new SqlDataAdapter(commandText, cn);
                cn.Open();
                dapst.Fill(dt);
                cn.Close();

                Logger.AddToFile("Process PKProductTax Data", storeID);
                if (TransferPOSTaxData(dt, "TAX200", false))
                {
                    Logger.AddToFile("Process pst to PKProductTax Data success", storeID);
                }
                else
                {
                    Logger.AddToFile("Process pst to PKProductTax Data failed", storeID);
                }

            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in UpdatePKProductTaxData:" + ex.Message, storeID);
            }
            finally
            {
                cn.Close();
            }
        }

        private bool TransferPOSTaxData(DataTable dtTaxProductName, string strTaxID, bool isDeleteAll)
        {
            Logger.AddToFile("TransferPOSTaxData start=======================", storeID);
            bool isUpdated = false;
            //int intCannotFindProductIDCounter = 0;
            //int intUpdateCounter = 0;
            //int intInsertCounter = 0;
            //int intErrorCounter = 0;

            SqlConnection cn = new SqlConnection();
            try
            {
                string strSelectSql = string.Empty;
                //string strTaxID = GetHSTTaxID();
                if (strTaxID == string.Empty)
                {
                    Logger.AddToFile("TransferPOSTaxData failed since cannot get TaxID", storeID);
                    return false;
                }
                strSelectSql = "SELECT [ID] as productid, cast('" + strTaxID + "' as nvarchar(50)) as taxid from pkproduct ";
                bool isFirstRow = true;
                int i = 0;
                foreach (DataRow dr in dtTaxProductName.Rows)
                {
                    if (isFirstRow)
                    {
                        strSelectSql += " where name1='" + (dr["productname"].ToString()).Replace("'", "''") + "' ";
                        isFirstRow = false;
                        i++;
                    }
                    else
                    {
                        i++;
                        strSelectSql += " or name1='" + (dr["productname"].ToString()).Replace("'", "''") + "' ";
                    }
                }

                DataTable dt = new DataTable();
                cn.ConnectionString = connectionString;
                SqlDataAdapter da = new SqlDataAdapter(strSelectSql, cn);
                cn.Open();
                da.Fill(dt);
                cn.Close();

                if (dt.Rows.Count == 0)
                {
                    Logger.AddToFile("No tax data in the datatable", storeID);
                    return false;
                }
                if (isDeleteAll)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = "delete from PKProductTax";
                    cn.Open();
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        Logger.AddToFile("Data deleted from PKProductTax", storeID);
                    }
                    else
                    {
                        Logger.AddToFile("Delete data from PKProductTax failed", storeID);
                        return false;
                    }
                    cn.Close();
                }
                SqlBulkCopy bulkcopy = new SqlBulkCopy(connectionString);
                bulkcopy.DestinationTableName = "PKProductTax";
                try
                {
                    bulkcopy.WriteToServer(dt);
                    isUpdated = true;
                }
                catch (Exception e)
                {
                    Logger.AddToFile("Error in TransferPOSTaxData bulkcopy:" + e.Message, storeID);
                    isUpdated = false;
                }
                Logger.AddToFile("TransferPOSTaxData success", storeID);
                return isUpdated;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in TransferPOSPriceData:" + ex.Message, storeID);
                isUpdated = false;
                return isUpdated;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
                Logger.AddToFile("TransferPOSTaxData end=======================", storeID);
            }
        }


        private string GetHSTTaxID()
        {
            string commandText = string.Empty; ;
            string strTaxID = string.Empty;
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection();
            try
            {
                commandText = "SELECT [ID] FROM [PKTax] WHERE [TaxName]='HST'";
                cn.ConnectionString = connectionString;
                SqlCommand cmd = new SqlCommand(commandText, cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    strTaxID = dr["ID"].ToString();
                }
                cn.Close();

                //Logger.AddToFile("Process PKProductTax Data", storeID);
                return strTaxID;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in UpdatePKProductTaxData:" + ex.Message, storeID);
                return string.Empty;
            }
            finally
            {
                cn.Close();
            }
        }
    }
}

