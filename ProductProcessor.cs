using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace PKCommon
{
    public class ProductProcessor
    {
        private string connectionString;
        //public string ConnectionString
        //{
        //    get { return connectionString; }
        //    set { connectionString = value; }
        //}

        private DataTable productDataTable;
        //public DataTable ProductDataTable
        //{
        //    get { return productDataTable; }
        //    set { productDataTable = value; }
        //}

        private string syncProductSequence;
        //public string SyncProductSequence
        //{
        //    get { return syncProductSequence; }
        //    set { syncProductSequence = value; }
        //}

        private string syncProductButtonColor;
        //public string SyncProductButtonColor
        //{
        //    get { return syncProductButtonColor; }
        //    set { syncProductButtonColor = value; }
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

        public ProductProcessor(string strConnectionString, DataTable dtProduct, bool serverFlag, string strStoreID)
        {
            this.connectionString = strConnectionString;
            this.productDataTable = dtProduct;
            this.isServer = serverFlag;
            this.storeID = strStoreID;
        }

        public ProductProcessor(string strConnectionString, DataTable dtProduct, bool serverFlag, string strStoreID, string strSyncProductSequence, string strSyncProductButtonColor)
        {
            this.connectionString = strConnectionString;
            this.productDataTable = dtProduct;

            this.isServer = serverFlag;
            this.storeID = strStoreID;
            if (strSyncProductSequence.Length == 0)
            {
                this.syncProductSequence = "true";//sync product syquence by default
            }
            else
            {
                this.syncProductSequence = strSyncProductSequence;
            }
            if (strSyncProductButtonColor.Length == 0)
            {
                this.syncProductButtonColor = "false";//does not sync product button color by default
            }
            else
            {
                this.syncProductButtonColor = strSyncProductButtonColor;
            }

        }

        /// <summary>
        /// update all product data to temp product table on the server
        /// </summary>
        /// <param name="productTable"></param>
        /// <returns></returns>
        public bool SynchronizeAllProductData2TempProductTable()
        {
            SqlConnection cn = new SqlConnection();
            try
            {
                //delete temp product data
                cn.ConnectionString = connectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "delete from producttemp";
                    cmd.Connection = cn;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                };
                //copy all the product data to the temp table
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    sqlbulkcopy.DestinationTableName = "producttemp";
                    sqlbulkcopy.WriteToServer(productDataTable);
                    sqlbulkcopy.Close();
                };
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in SynchronizeAllProductData2TempProductTable:" + ex.Message);
                return false;
            }
            finally
            {
                cn.Close();
            }
        }

        /// <summary>
        /// Process local product/PKProduct data by the temp product data
        /// </summary>
        public void ProcessProductData()
        {
            try
            {
                Logger.AddToFile("DeleteRedundantProductData");
                DeleteRedundantProductData();

                if (isServer)
                {
                    Logger.AddToFile("MarkDeletedRedundantPKProductData");
                    MarkDeletedRedundantPKProductData();
                }

                Logger.AddToFile("Update/Insert ProductData");
                UpdateProductData();

                if (isServer)
                {
                    Logger.AddToFile("Update/Insert PKDepartment Data ==================");
                    UpdatePKDepartmentData();

                    Logger.AddToFile("Update/Insert PKCategory Data ====================");
                    UpdatePKCategoryData();

                    Logger.AddToFile("Update/Insert PKProduct Data =====================");
                    UpdatePKProductData();
                }
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in ProcessProductData: =====================");
                Logger.AddToFile("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Delete local Redundant Product Data accroding to the data in producttemp
        /// </summary>
        /// <param name="strConnectionString"></param>
        private void DeleteRedundantProductData()
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
                cmdTargetSelect.CommandText = "select productitemid from product";

                drTarget = cmdTargetSelect.ExecuteReader();
                int updatedRowCount = 0;
                while (drTarget.Read())
                {
                    string tempIndex = drTarget[0].ToString();
                    if (tempIndex.Length == 0)
                    {
                        continue;
                    }
                    SqlCommand cmdSourceSelect = new SqlCommand();
                    cmdSourceSelect.Connection = cnnSource;
                    cmdSourceSelect.CommandText = "select productitemid from producttemp where productitemid='" + tempIndex + "'";

                    using (SqlDataReader drSource = cmdSourceSelect.ExecuteReader())
                    {
                        if (!drSource.HasRows)
                        {
                            SqlCommand cmdTargetDelete = new SqlCommand();
                            cmdTargetDelete.Connection = cnnTargetDelete;
                            cmdTargetDelete.CommandText = "delete from product where productitemid='" + tempIndex + "'";

                            cnnTargetDelete.Open();
                            cmdTargetDelete.ExecuteNonQuery();
                            cnnTargetDelete.Close();
                            updatedRowCount++;
                        }
                    };

                }
                cnnSource.Close();
                cnnTarget.Close();
                Logger.AddToFile(updatedRowCount.ToString() + " rows product data have been removed.");
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in DeleteRedundantProductData: " + ex.Message);
            }
            finally
            {
                cnnSource.Close();
                cnnTarget.Close();
            }

        }

        /// <summary>
        /// mark the redundant PKProduct data as deleted
        /// </summary>
        private void MarkDeletedRedundantPKProductData()
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
                cmdTargetSelect.CommandText = "select plu from pkproduct";

                drTarget = cmdTargetSelect.ExecuteReader();
                int updatedRowCount = 0;
                while (drTarget.Read())
                {
                    string tempIndex = drTarget[0].ToString();
                    if (tempIndex.Length == 0)
                    {
                        continue;
                    }
                    SqlCommand cmdSourceSelect = new SqlCommand();
                    cmdSourceSelect.Connection = cnnSource;
                    cmdSourceSelect.CommandText = "select productitemid from producttemp where productitemid='" + tempIndex + "'";

                    using (SqlDataReader drSource = cmdSourceSelect.ExecuteReader())
                    {
                        if (!drSource.HasRows)
                        {
                            SqlCommand cmdTargetUpdate = new SqlCommand();
                            cmdTargetUpdate.Connection = cnnTargetDelete;
                            cmdTargetUpdate.CommandText = "update pkproduct set status='Deleted' where plu='" + tempIndex + "'";

                            cnnTargetDelete.Open();
                            cmdTargetUpdate.ExecuteNonQuery();
                            cnnTargetDelete.Close();
                            updatedRowCount++;
                        }
                    };

                }
                cnnSource.Close();
                cnnTarget.Close();
                Logger.AddToFile(updatedRowCount.ToString() + " rows PKProduct data have been marked as deleted.");
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in MarkDeletedRedundantPKProductData: " + ex.Message);
            }
            finally
            {
                cnnSource.Close();
                cnnTarget.Close();
            }

        }

        /// <summary>
        /// Product: POS - POS
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="strDataStream"></param>
        /// <returns></returns>
        private bool UpdateProductData()
        {
            bool isUpdated = false;
            int intErrorCounter = 0;
            int intInstertedCounter = 0;
            int intUpdatedCounter = 0;
            //int intSkipOverwriteCounter = 0;
            SqlConnection sqlCnn = new SqlConnection();
            try
            {
                string productname = string.Empty;
                string productdisplay = string.Empty;
                string plu = string.Empty;
                string productunit = string.Empty;
                string productupper = string.Empty;
                int productsequence = 10000;
                int productfastsequence = 0;
                string productdiscountable = "yes";
                string status = "available";
                string isopensale = "no";
                decimal productinstock = 0.00m;
                decimal productfrequency = 0m;
                string producttype = string.Empty;
                string storeid = string.Empty;
                string mixandmatchid = string.Empty;
                decimal productminstock = 10.00m;
                decimal productmaxstock = 999999.00m;
                string productremarks = string.Empty;
                string createdby = "Imported";
                string createdtime = DateTime.Now.ToString();
                string updatedby = "Imported";
                string updatedtime = DateTime.Now.ToString();
                decimal producttearminus = 0.00m;
                string productotherlanguagedisplay = string.Empty;
                string productbuttoncolor = string.Empty;
                string productwarranty = string.Empty;
                string productsupplier = string.Empty;
                decimal productcost = 0.00m;
                int productmarkup = 0;
                string productpicture = string.Empty;
                string productzone = string.Empty;
                string productinvcontrol = "no";
                string productbrand = string.Empty;
                string productsize = string.Empty;
                string productitemid = string.Empty;
                string productdimension = string.Empty;
                int productqtyinbox = 1;
                string productcateid = string.Empty;
                string productstocknumber = string.Empty;
                decimal productcommrate = 0.00m;
                string productispackage = "no";
                decimal productpointsrate = 1.00m;
                string productoverwrite = "yes";
                string priceoverwrite = "yes";

                Logger.AddToFile("Procressing porduct data.");

                if (productDataTable.Rows.Count <= 0)
                {
                    Logger.AddToFile("No data for product.");
                    return false;
                }

                Logger.AddToFile("Setup database connection for product data update!");
                sqlCnn.ConnectionString = connectionString;
                sqlCnn.Open();

                for (int i = 0; i < productDataTable.Rows.Count; i++)
                {
                    //Logger.AddToFile("i=" + i);
                    productname = productDataTable.Rows[i]["productname"].ToString();
                    productdisplay = productDataTable.Rows[i]["productdisplay"].ToString();
                    plu = productDataTable.Rows[i]["plu"].ToString();
                    productunit = productDataTable.Rows[i]["productunit"].ToString();
                    productupper = productDataTable.Rows[i]["productupper"].ToString();
                    if (string.Equals(syncProductSequence, "true", StringComparison.CurrentCultureIgnoreCase))
                    {
                        int.TryParse(productDataTable.Rows[i]["productsequence"].ToString(), out productsequence);
                        //Logger.AddToFile("productsequence=" + productDataTable.Rows[i]["productsequence"].ToString() + ":" + productsequence.ToString());
                    }
                    //productfastsequence = productDataTable.Rows[i]["productfastsequence"].ToString();
                    productdiscountable = productDataTable.Rows[i]["productdiscountable"].ToString();
                    status = productDataTable.Rows[i]["status"].ToString();
                    isopensale = productDataTable.Rows[i]["isopensale"].ToString();
                    //productinstock = productDataTable.Rows[i]["productinstock"].ToString();
                    //productfrequency = productDataTable.Rows[i]["productfrequency"].ToString();
                    producttype = productDataTable.Rows[i]["producttype"].ToString();
                    storeid = this.storeID;
                    mixandmatchid = productDataTable.Rows[i]["mixandmatchid"].ToString();
                    //productminstock = productDataTable.Rows[i]["productminstock"].ToString();
                    //productmaxstock = productDataTable.Rows[i]["productmaxstock"].ToString();
                    productremarks = productDataTable.Rows[i]["productremarks"].ToString();
                    createdby = productDataTable.Rows[i]["createdby"].ToString();
                    createdtime = productDataTable.Rows[i]["createdtime"].ToString();
                    updatedby = "PKService";// productDataTable.Rows[i]["updatedby"].ToString();
                    updatedtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.000");// productDataTable.Rows[i]["updatedtime"].ToString();
                    //producttearminus = productDataTable.Rows[i]["producttearminus"].ToString();
                    productotherlanguagedisplay = productDataTable.Rows[i]["productotherlanguagedisplay"].ToString();
                    productbuttoncolor = productDataTable.Rows[i]["productbuttoncolor"].ToString();
                    productwarranty = productDataTable.Rows[i]["productwarranty"].ToString();
                    productsupplier = productDataTable.Rows[i]["productsupplier"].ToString();
                    //productcost = productDataTable.Rows[i]["productcost"].ToString();
                    //productmarkup = productDataTable.Rows[i]["productmarkup"].ToString();
                    productpicture = productDataTable.Rows[i]["productpicture"].ToString();
                    productzone = productDataTable.Rows[i]["productzone"].ToString();
                    productinvcontrol = productDataTable.Rows[i]["productinvcontrol"].ToString();
                    productbrand = productDataTable.Rows[i]["productbrand"].ToString();
                    productsize = productDataTable.Rows[i]["productsize"].ToString();
                    productitemid = productDataTable.Rows[i]["productitemid"].ToString();
                    productdimension = productDataTable.Rows[i]["productdimension"].ToString();
                    //productqtyinbox = productDataTable.Rows[i]["productqtyinbox"].ToString();
                    productcateid = productDataTable.Rows[i]["productcateid"].ToString();
                    productstocknumber = productDataTable.Rows[i]["productstocknumber"].ToString();
                    //productcommrate = productDataTable.Rows[i]["productcommrate"].ToString();
                    productispackage = productDataTable.Rows[i]["productispackage"].ToString();
                    //productpointsrate = productDataTable.Rows[i]["productpointsrate"].ToString();
                    productoverwrite = productDataTable.Rows[i]["productoverwrite"].ToString(); ;
                    priceoverwrite = productDataTable.Rows[i]["priceoverwrite"].ToString(); ;

                    //if (plu == string.Empty)
                    //{
                    //    Logger.AddToFile("Item: " + productname + " was not updated since it does not have PLU.");
                    //    continue;
                    //}

                    //Logger.AddToFile("Setup parameters for product data update.");
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlCnn;

                    //check exist
                    SqlCommand cmdDistinct = new SqlCommand();
                    cmdDistinct.Connection = sqlCnn;
                    //by product name===================================
                    //cmdDistinct.CommandText = "select [productname] from product where [productname]=@productname";
                    //cmdDistinct.Parameters.Add("@productname", SqlDbType.NVarChar, 150).Value = productname;

                    //by product itmeid===================================
                    cmdDistinct.CommandText = "select [productitemid],[productfastsequence],[productbuttoncolor] from product where [productitemid]=@productitemid";
                    cmdDistinct.Parameters.Add("@productitemid", SqlDbType.NVarChar, 150).Value = productitemid;
                    //cmdDistinct.Parameters.Add("@status", SqlDbType.NVarChar, 50).Value = status;

                    SqlDataReader dr = cmdDistinct.ExecuteReader();
                    string strProductfastsequence = string.Empty;
                    bool isHasRows = false;
                    while (dr.Read())
                    {
                        isHasRows = true;
                        //Logger.AddToFile("isHasRows");
                        if (!isServer)//server sync all button color
                        {
                            strProductfastsequence = dr["productfastsequence"].ToString();
                            if (strProductfastsequence != "0")
                            {
                                //fast move buttons don't sync button color
                                //Logger.AddToFile("strProductfastsequence:" + strProductfastsequence);
                                productbuttoncolor = dr["productbuttoncolor"].ToString();
                            }
                        }
                    }
                    cmdDistinct.Dispose();
                    dr.Close();
                    if (isHasRows)
                    {
                        intUpdatedCounter++;
                        //by product name===================================
                        //Logger.AddToFile("Update productname=" + productname);
                        //by product itmeid===================================
                        //Logger.AddToFile("Update productitemid=" + productitemid);

                        cmd.CommandText = "update product set ";
                        cmd.CommandText += "productdisplay=@productdisplay, ";
                        cmd.CommandText += "plu=@plu, ";
                        cmd.CommandText += "productunit=@productunit, ";
                        cmd.CommandText += "productupper=@productupper, ";
                        cmd.CommandText += "productsequence=@productsequence, ";
                        //cmd.CommandText += "productfastsequence=@productfastsequence, ";
                        cmd.CommandText += "productdiscountable=@productdiscountable, ";
                        cmd.CommandText += "status=@status, ";
                        cmd.CommandText += "isopensale=@isopensale, ";
                        //cmd.CommandText += "productinstock=@productinstock, ";
                        //cmd.CommandText += "productfrequency=@productfrequency, ";
                        cmd.CommandText += "producttype=@producttype, ";
                        cmd.CommandText += "storeid=@storeid, ";
                        cmd.CommandText += "mixandmatchid=@mixandmatchid, ";
                        //cmd.CommandText += "productminstock=@productminstock, ";
                        //cmd.CommandText += "productmaxstock=@productmaxstock, ";
                        cmd.CommandText += "productremarks=@productremarks, ";
                        cmd.CommandText += "createdby=@createdby, ";
                        cmd.CommandText += "createdtime=@createdtime, ";
                        cmd.CommandText += "updatedby=@updatedby, ";
                        cmd.CommandText += "updatedtime=@updatedtime, ";
                        cmd.CommandText += "producttearminus=@producttearminus, ";
                        cmd.CommandText += "productotherlanguagedisplay=@productotherlanguagedisplay, ";
                        if (string.Equals(syncProductButtonColor, "true", StringComparison.CurrentCultureIgnoreCase))
                        {
                            cmd.CommandText += "productbuttoncolor=@productbuttoncolor, ";
                        }
                        cmd.CommandText += "productwarranty=@productwarranty, ";
                        cmd.CommandText += "productsupplier=@productsupplier, ";
                        cmd.CommandText += "productcost=@productcost, ";
                        cmd.CommandText += "productmarkup=@productmarkup, ";
                        cmd.CommandText += "productpicture=@productpicture, ";
                        cmd.CommandText += "productzone=@productzone, ";
                        cmd.CommandText += "productinvcontrol=@productinvcontrol, ";
                        cmd.CommandText += "productbrand=@productbrand, ";
                        cmd.CommandText += "productsize=@productsize, ";
                        cmd.CommandText += "productdimension=@productdimension, ";
                        cmd.CommandText += "productqtyinbox=@productqtyinbox, ";
                        cmd.CommandText += "productcateid=@productcateid, ";
                        cmd.CommandText += "productstocknumber=@productstocknumber, ";
                        cmd.CommandText += "productcommrate=@productcommrate, ";
                        cmd.CommandText += "productispackage=@productispackage, ";
                        cmd.CommandText += "productpointsrate=@productpointsrate, ";
                        cmd.CommandText += "productoverwrite=@productoverwrite, ";
                        cmd.CommandText += "priceoverwrite=@priceoverwrite, ";
                        //by product name===================================
                        //cmd.CommandText += "productitemid=@productitemid where productname=@productname";
                        //by product itmeid===================================
                        cmd.CommandText += "productname=@productname where productitemid=@productitemid";
                    }
                    else
                    {
                        intInstertedCounter++;
                        //by product name===================================
                        //Logger.AddToFile("Insert productname=" + productname);
                        //by product itmeid===================================
                        //Logger.AddToFile("Insert productitemid=" + productitemid);
                        string strFields = string.Empty;
                        string strValues = string.Empty;
                        strFields = "productname, ";
                        strValues = "@productname, ";
                        strFields += "productdisplay, ";
                        strValues += "@productdisplay, ";
                        strFields += "plu, ";
                        strValues += "@plu, ";
                        strFields += "productunit, ";
                        strValues += "@productunit, ";
                        strFields += "productupper, ";
                        strValues += "@productupper, ";
                        strFields += "productsequence, ";
                        strValues += "@productsequence, ";
                        strFields += "productfastsequence, ";
                        strValues += "@productfastsequence, ";
                        strFields += "productdiscountable, ";
                        strValues += "@productdiscountable, ";
                        strFields += "status, ";
                        strValues += "@status, ";
                        strFields += "isopensale, ";
                        strValues += "@isopensale, ";
                        strFields += "productinstock, ";
                        strValues += "@productinstock, ";
                        strFields += "productfrequency, ";
                        strValues += "@productfrequency, ";
                        strFields += "producttype, ";
                        strValues += "@producttype, ";
                        strFields += "storeid, ";
                        strValues += "@storeid, ";
                        strFields += "mixandmatchid, ";
                        strValues += "@mixandmatchid, ";
                        strFields += "productminstock, ";
                        strValues += "@productminstock, ";
                        strFields += "productmaxstock, ";
                        strValues += "@productmaxstock, ";
                        strFields += "productremarks, ";
                        strValues += "@productremarks, ";
                        strFields += "createdby, ";
                        strValues += "@createdby, ";
                        strFields += "createdtime, ";
                        strValues += "@createdtime, ";
                        strFields += "updatedby, ";
                        strValues += "@updatedby, ";
                        strFields += "updatedtime, ";
                        strValues += "@updatedtime, ";
                        strFields += "producttearminus, ";
                        strValues += "@producttearminus, ";
                        strFields += "productotherlanguagedisplay, ";
                        strValues += "@productotherlanguagedisplay, ";
                        strFields += "productbuttoncolor, ";
                        strValues += "@productbuttoncolor, ";
                        strFields += "productwarranty, ";
                        strValues += "@productwarranty, ";
                        strFields += "productsupplier, ";
                        strValues += "@productsupplier, ";
                        strFields += "productcost, ";
                        strValues += "@productcost, ";
                        strFields += "productmarkup, ";
                        strValues += "@productmarkup, ";
                        strFields += "productpicture, ";
                        strValues += "@productpicture, ";
                        strFields += "productzone, ";
                        strValues += "@productzone, ";
                        strFields += "productinvcontrol, ";
                        strValues += "@productinvcontrol, ";
                        strFields += "productbrand, ";
                        strValues += "@productbrand, ";
                        strFields += "productsize, ";
                        strValues += "@productsize, ";
                        strFields += "productitemid, ";
                        strValues += "@productitemid, ";
                        strFields += "productdimension, ";
                        strValues += "@productdimension, ";
                        strFields += "productqtyinbox, ";
                        strValues += "@productqtyinbox, ";
                        strFields += "productcateid, ";
                        strValues += "@productcateid, ";
                        strFields += "productstocknumber, ";
                        strValues += "@productstocknumber, ";
                        strFields += "productcommrate, ";
                        strValues += "@productcommrate, ";
                        strFields += "productispackage, ";
                        strValues += "@productispackage, ";
                        strFields += "productpointsrate, ";
                        strValues += "@productpointsrate, ";
                        strFields += "productoverwrite, ";
                        strValues += "@productoverwrite, ";
                        strFields += "priceoverwrite";
                        strValues += "@priceoverwrite";

                        cmd.CommandText = "insert into product (" + strFields + ") values (" + strValues + ")";

                    }

                    cmd.Parameters.Add("@productname", SqlDbType.NVarChar).Value = productname;
                    cmd.Parameters.Add("@productdisplay", SqlDbType.NVarChar).Value = productdisplay;
                    cmd.Parameters.Add("@plu", SqlDbType.NVarChar).Value = plu;
                    cmd.Parameters.Add("@productunit", SqlDbType.NVarChar).Value = productunit;
                    cmd.Parameters.Add("@productupper", SqlDbType.NVarChar).Value = productupper;
                    cmd.Parameters.Add("@productsequence", SqlDbType.Int).Value = productsequence;
                    cmd.Parameters.Add("@productfastsequence", SqlDbType.Int).Value = productfastsequence;
                    cmd.Parameters.Add("@productdiscountable", SqlDbType.NVarChar).Value = productdiscountable;
                    cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = status;
                    cmd.Parameters.Add("@isopensale", SqlDbType.NVarChar).Value = isopensale;
                    cmd.Parameters.Add("@productinstock", SqlDbType.Decimal).Value = productinstock;
                    cmd.Parameters.Add("@productfrequency", SqlDbType.Decimal).Value = productfrequency;
                    cmd.Parameters.Add("@producttype", SqlDbType.NVarChar).Value = producttype;
                    cmd.Parameters.Add("@storeid", SqlDbType.NVarChar).Value = storeid;
                    cmd.Parameters.Add("@mixandmatchid", SqlDbType.NVarChar).Value = mixandmatchid;
                    cmd.Parameters.Add("@productminstock", SqlDbType.Decimal).Value = productminstock;
                    cmd.Parameters.Add("@productmaxstock", SqlDbType.Decimal).Value = productmaxstock;
                    cmd.Parameters.Add("@productremarks", SqlDbType.NVarChar).Value = productremarks;
                    cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = createdby;
                    cmd.Parameters.Add("@createdtime", SqlDbType.DateTime).Value = createdtime;
                    cmd.Parameters.Add("@updatedby", SqlDbType.NVarChar).Value = updatedby;
                    cmd.Parameters.Add("@updatedtime", SqlDbType.DateTime).Value = updatedtime;
                    cmd.Parameters.Add("@producttearminus", SqlDbType.Decimal).Value = producttearminus;
                    cmd.Parameters.Add("@productotherlanguagedisplay", SqlDbType.NVarChar).Value = productotherlanguagedisplay;
                    cmd.Parameters.Add("@productbuttoncolor", SqlDbType.NVarChar).Value = productbuttoncolor;
                    cmd.Parameters.Add("@productwarranty", SqlDbType.NVarChar).Value = productwarranty;
                    cmd.Parameters.Add("@productsupplier", SqlDbType.NVarChar).Value = productsupplier;
                    cmd.Parameters.Add("@productcost", SqlDbType.Decimal).Value = productcost;
                    cmd.Parameters.Add("@productmarkup", SqlDbType.Int).Value = productmarkup;
                    cmd.Parameters.Add("@productpicture", SqlDbType.NVarChar).Value = productpicture;
                    cmd.Parameters.Add("@productzone", SqlDbType.NVarChar).Value = productzone;
                    cmd.Parameters.Add("@productinvcontrol", SqlDbType.NVarChar).Value = productinvcontrol;
                    cmd.Parameters.Add("@productbrand", SqlDbType.NVarChar).Value = productbrand;
                    cmd.Parameters.Add("@productsize", SqlDbType.NVarChar).Value = productsize;
                    cmd.Parameters.Add("@productitemid", SqlDbType.NVarChar).Value = productitemid;
                    cmd.Parameters.Add("@productdimension", SqlDbType.NVarChar).Value = productdimension;
                    cmd.Parameters.Add("@productqtyinbox", SqlDbType.Int).Value = productqtyinbox;
                    cmd.Parameters.Add("@productcateid", SqlDbType.NVarChar).Value = productcateid;
                    cmd.Parameters.Add("@productstocknumber", SqlDbType.NVarChar).Value = productstocknumber;
                    cmd.Parameters.Add("@productcommrate", SqlDbType.Decimal).Value = productcommrate;
                    cmd.Parameters.Add("@productispackage", SqlDbType.NVarChar).Value = productispackage;
                    cmd.Parameters.Add("@productpointsrate", SqlDbType.Decimal).Value = productpointsrate;
                    cmd.Parameters.Add("@productoverwrite", SqlDbType.NVarChar).Value = productoverwrite;
                    cmd.Parameters.Add("@priceoverwrite", SqlDbType.NVarChar).Value = priceoverwrite;

                    //Logger.AddToFile("Begin execute product data.");

                    int intRows = cmd.ExecuteNonQuery();
                    if (intRows > 0)
                    {
                        isUpdated = true;
                        //by product name===================================
                        //Logger.AddToFile("Updated productname=" + productname);
                        //by product itmeid===================================
                        //Logger.AddToFile("Updated productitemid=" + productitemid);
                    }
                    else
                    {
                        intErrorCounter++;
                        isUpdated = false;
                        //by product name===================================
                        //Logger.AddToFile("Update failed productname=" + productname);
                        //by product itmeid===================================
                        //Logger.AddToFile("Update failed productitemid=" + productitemid);
                    }
                }
                sqlCnn.Close();

                return isUpdated;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in UpdateProductData:" + ex.Message);
                isUpdated = false;
                return isUpdated;
            }
            finally
            {
                if (sqlCnn.State == ConnectionState.Open)
                {
                    sqlCnn.Close();
                }
                Logger.AddToFile(string.Format("The intErrorCounter={0}", intErrorCounter));
                Logger.AddToFile(string.Format("The intInstertedCounter={0}", intInstertedCounter));
                Logger.AddToFile(string.Format("The intUpdatedCounter={0}", intUpdatedCounter));
                //Logger.AddToFile(string.Format("The intSkipOverwriteCounter={0}", intSkipOverwriteCounter));
            }
        }

        private void UpdatePKDepartmentData()
        {
            string strError = string.Empty;
            DataTable dt = GetPOSModuleData(POSModuleNames.POSDepartment, out strError);
            if (strError.Length > 0)
            {
                Logger.AddToFile("UpdatePKDepartmentData: Get Department Data Error: " + strError);
                return;
            }
            Logger.AddToFile("Process PKDepartment Data");
            if (TransferPOSDepartmentData(dt))
            {
                Logger.AddToFile("Process PKDepartment Data success");
            }
            else
            {
                Logger.AddToFile("Process PKDepartment Data failed");
            }
        }

        private bool TransferPOSDepartmentData(DataTable dtDepartment)
        {
            bool isUpdated = false;
            int intUpdateCounter = 0;
            int intInsertCounter = 0;
            int intErrorCounter = 0;
            SqlConnection sqlCnn = new SqlConnection();
            try
            {
                //SELECT DepartmentID, DepartmentPLU, DepartmentName, DepartmentRemarks FROM PKDepartment
                string strDepartmentPLU = string.Empty;
                string strDepartmentName = string.Empty;
                string strDepartmentOtherName = string.Empty;
                string strDepartmentRemarks = string.Empty;
                string strDepartmentCreateTime = string.Empty;
                string strDepartmentUpdateTime = string.Empty;

                Logger.AddToFile("Procressing POS Department data.");

                if (dtDepartment.Rows.Count <= 0)
                {
                    Logger.AddToFile("No data for POS Department.");
                    return false;
                }

                Logger.AddToFile("Setup database connection for POS Department data update!");
                sqlCnn.ConnectionString = connectionString;
                sqlCnn.Open();

                for (int rowIndex = 0; rowIndex < dtDepartment.Rows.Count; rowIndex++)
                {
                    strDepartmentPLU = dtDepartment.Rows[rowIndex]["plu"].ToString();
                    strDepartmentName = dtDepartment.Rows[rowIndex]["productname"].ToString();
                    strDepartmentOtherName = dtDepartment.Rows[rowIndex]["productotherlanguagedisplay"].ToString();
                    strDepartmentRemarks = dtDepartment.Rows[rowIndex]["productremarks"].ToString();
                    DateTime tempDT;
                    DateTime.TryParse(dtDepartment.Rows[rowIndex]["createdtime"].ToString(), out tempDT);
                    strDepartmentCreateTime = tempDT.ToString(GlobalConst.strTimeStampFormat);
                    DateTime.TryParse(dtDepartment.Rows[rowIndex]["updatedtime"].ToString(), out tempDT);
                    strDepartmentUpdateTime = tempDT.ToString(GlobalConst.strTimeStampFormat);

                    //if (plu == string.Empty)
                    //{
                    //    Logger.AddToFile("Item: " + productname + " was not updated since it does not have PLU.");
                    //    continue;
                    //}
                    //Logger.AddToFile("Setup parameters for POS Department data update.");

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlCnn;

                    //check exist
                    using (SqlCommand cmdDistinct = new SqlCommand())
                    {
                        cmdDistinct.Connection = sqlCnn;
                        cmdDistinct.CommandText = "select [PLU] from PKDepartment where [PLU]=@PLU";
                        cmdDistinct.Parameters.AddWithValue("@PLU", strDepartmentPLU);
                        using (SqlDataReader dr = cmdDistinct.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                intUpdateCounter++;
                                //Logger.AddToFile("Update DepartmentPLU=" + strDepartmentPLU);
                                cmd.CommandText = "update PKDepartment set Name=@Name, OtherName=@OtherName, Remarks=@Remarks, CreateTime=@CreateTime, UpdateTime=@UpdateTime where PLU=@PLU";
                            }
                            else
                            {
                                intInsertCounter++;
                                //Logger.AddToFile("Insert DepartmentPLU=" + strDepartmentPLU);
                                cmd.CommandText = "insert into PKDepartment (ID, PLU, Name, OtherName, Remarks, CreateTime, UpdateTime) values (@ID, @PLU, @Name, @OtherName, @Remarks, @CreateTime, @UpdateTime)";
                                cmd.Parameters.AddWithValue("@ID", Guid.NewGuid().ToString());
                            }
                        }
                        //cmdDistinct.Dispose();
                        //dr.Close();
                    };
                    //(@ID, @PLU, @Name, @OtherName, @Remarks, @CreateTime, @UpdateTime)
                    cmd.Parameters.AddWithValue("@PLU", strDepartmentPLU);
                    cmd.Parameters.AddWithValue("@Name", strDepartmentName);
                    cmd.Parameters.AddWithValue("@OtherName", strDepartmentOtherName);
                    cmd.Parameters.AddWithValue("@Remarks", strDepartmentRemarks);
                    cmd.Parameters.AddWithValue("@CreateTime", strDepartmentCreateTime);
                    cmd.Parameters.AddWithValue("@UpdateTime", strDepartmentUpdateTime);

                    //Logger.AddToFile("Begin execute.");

                    int intRows = cmd.ExecuteNonQuery();
                    if (intRows > 0)
                    {
                        isUpdated = true;
                        //Logger.AddToFile("Updated DepartmentPLU=" + strDepartmentPLU);
                    }
                    else
                    {
                        isUpdated = false;
                        intErrorCounter++;
                        //Logger.AddToFile("Update failed DepartmentPLU=" + strDepartmentPLU);
                    }
                }
                sqlCnn.Close();
                return isUpdated;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in UpdateProductData:" + ex.Message);
                isUpdated = false;
                return isUpdated;
            }
            finally
            {
                if (sqlCnn.State == ConnectionState.Open)
                {
                    sqlCnn.Close();
                }
                Logger.AddToFile(string.Format("The intErrorCounter={0}", intErrorCounter));
                Logger.AddToFile(string.Format("The intInsertCounter={0}", intInsertCounter));
                Logger.AddToFile(string.Format("The intUpdateCounter={0}", intUpdateCounter));
            }
        }

        private void UpdatePKCategoryData()
        {
            string strError = string.Empty;
            DataTable dt = GetPOSModuleData(POSModuleNames.POSCategory, out strError);
            if (strError.Length > 0)
            {
                Logger.AddToFile("UpdatePKCategoryData: Get Category Data Error: " + strError);
                return;
            }
            Logger.AddToFile("Process PKCategory Data");
            if (TransferPOSCategoryData(dt))
            {
                Logger.AddToFile("Process PKCategory Data success");
            }
            else
            {
                Logger.AddToFile("Process PKCategory Data failed");
            }
        }

        private bool TransferPOSCategoryData(DataTable dtCategroy)
        {
            bool isUpdated = false;
            int intUpdateCounter = 0;
            int intInsertCounter = 0;
            int intErrorCounter = 0;
            SqlConnection sqlCnn = new SqlConnection();
            try
            {
                //SELECT CategoryID, CategoryPLU, CategoryName, CategoryDepartmentID, 
                //CategoryRemarks FROM PKCategory
                string strCategoryPLU = string.Empty;
                string strCategoryName = string.Empty;
                string strCategoryOtherName = string.Empty;
                string strCategoryRemarks = string.Empty;
                string strCategoryCreateTime = string.Empty;
                string strCategoryUpdateTime = string.Empty;
                string strDepartmentID = string.Empty;

                Logger.AddToFile("Procressing POS Category data.");
                //DataTable dtTemp;
                if (dtCategroy.Rows.Count <= 0)
                {
                    Logger.AddToFile("No data for POS Category.");
                    return false;
                }

                Logger.AddToFile("Setup database connection for POS Category data update!");
                sqlCnn.ConnectionString = connectionString;
                sqlCnn.Open();

                for (int rowIndex = 0; rowIndex < dtCategroy.Rows.Count; rowIndex++)
                {
                    strCategoryPLU = dtCategroy.Rows[rowIndex]["plu"].ToString();
                    strCategoryName = dtCategroy.Rows[rowIndex]["productname"].ToString();
                    strCategoryOtherName = dtCategroy.Rows[rowIndex]["productotherlanguagedisplay"].ToString();
                    strCategoryRemarks = dtCategroy.Rows[rowIndex]["productremarks"].ToString();
                    DateTime tempDT;
                    DateTime.TryParse(dtCategroy.Rows[rowIndex]["createdtime"].ToString(), out tempDT);
                    strCategoryCreateTime = tempDT.ToString(GlobalConst.strTimeStampFormat);
                    DateTime.TryParse(dtCategroy.Rows[rowIndex]["updatedtime"].ToString(), out tempDT);
                    strCategoryUpdateTime = tempDT.ToString(GlobalConst.strTimeStampFormat);

                    string strDepartmentNmae = dtCategroy.Rows[rowIndex]["productupper"].ToString();
                    using (SqlCommand cmdDepartmentID = new SqlCommand())
                    {
                        cmdDepartmentID.CommandText = "SELECT ID FROM PKDepartment WHERE (Name = '" + strDepartmentNmae + "')";
                        cmdDepartmentID.Connection = sqlCnn;
                        SqlDataReader drDepartmentID = cmdDepartmentID.ExecuteReader();
                        while (drDepartmentID.Read())
                        {
                            strDepartmentID = drDepartmentID[0].ToString();
                        }
                        drDepartmentID.Close();
                    }
                    //if (plu == string.Empty)
                    //{
                    //    Logger.AddToFile("Item: " + productname + " was not updated since it does not have PLU.");
                    //    continue;
                    //}
                    //Logger.AddToFile("Setup parameters for POS Category data update.");

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlCnn;

                    //check exist
                    using (SqlCommand cmdDistinct = new SqlCommand())
                    {
                        cmdDistinct.Connection = sqlCnn;
                        cmdDistinct.CommandText = "select [PLU] from PKCategory where [PLU]=@PLU";
                        cmdDistinct.Parameters.AddWithValue("@PLU", strCategoryPLU);
                        SqlDataReader dr = cmdDistinct.ExecuteReader();
                        if (dr.HasRows)
                        {
                            intUpdateCounter++;
                            //Logger.AddToFile("Update CategoryPLU=" + strCategoryPLU);
                            cmd.CommandText = "update PKCategory set DepartmentID=@DepartmentID, Name=@Name, OtherName=@OtherName, Remarks=@Remarks, CreateTime=@CreateTime, UpdateTime=@UpdateTime where PLU=@PLU";
                        }
                        else
                        {
                            intInsertCounter++;
                            //Logger.AddToFile("Insert CategoryPLU=" + strCategoryPLU);
                            cmd.CommandText = "insert into PKCategory (ID, DepartmentID, PLU, Name, OtherName, Remarks, CreateTime, UpdateTime) values (@ID, @DepartmentID, @PLU, @Name, @OtherName, @Remarks, @CreateTime, @UpdateTime)";
                            cmd.Parameters.AddWithValue("@ID", Guid.NewGuid().ToString());
                        }
                        dr.Close();
                    }

                    cmd.Parameters.AddWithValue("@DepartmentID", strDepartmentID);
                    cmd.Parameters.AddWithValue("@PLU", strCategoryPLU);
                    cmd.Parameters.AddWithValue("@Name", strCategoryName);
                    cmd.Parameters.AddWithValue("@OtherName", strCategoryOtherName);
                    cmd.Parameters.AddWithValue("@Remarks", strCategoryRemarks);
                    cmd.Parameters.AddWithValue("@CreateTime", strCategoryCreateTime);
                    cmd.Parameters.AddWithValue("@UpdateTime", strCategoryUpdateTime);

                    //Logger.AddToFile("Begin execute.");

                    int intRows = cmd.ExecuteNonQuery();
                    if (intRows > 0)
                    {
                        isUpdated = true;
                        //Logger.AddToFile("Updated CategoryPLU=" + strCategoryPLU);
                    }
                    else
                    {
                        isUpdated = false;
                        intErrorCounter++;
                        //Logger.AddToFile("Update failed CategoryPLU=" + strCategoryPLU);
                    }
                }
                sqlCnn.Close();
                return isUpdated;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in TransferPOSCategoryData:" + ex.Message);
                isUpdated = false;
                return isUpdated;
            }
            finally
            {
                if (sqlCnn.State == ConnectionState.Open)
                {
                    sqlCnn.Close();
                }
                Logger.AddToFile(string.Format("The intErrorCounter={0}", intErrorCounter));
                Logger.AddToFile(string.Format("The intInsertCounter={0}", intInsertCounter));
                Logger.AddToFile(string.Format("The intUpdateCounter={0}", intUpdateCounter));
            }
        }

        private void UpdatePKProductData()
        {
            string strError = string.Empty;
            DataTable dt = GetPOSModuleData(POSModuleNames.POSProduct, out strError);
            if (strError.Length > 0)
            {
                Logger.AddToFile("UpdatePKProductData: Get Product Data Error: " + strError);
                return;
            }
            Logger.AddToFile("Process PKProduct Data");
            if (TransferPOSProductData(dt))
            {
                Logger.AddToFile("Process PKProduct Data success");
            }
            else
            {
                Logger.AddToFile("Process PKProduct Data failed");
            }
        }

        private bool TransferPOSProductData(DataTable dtProduct)
        {
            bool isUpdated = false;
            //int intCannotFindCategoryID = 0;
            int intUpdateCounter = 0;
            int intInsertCounter = 0;
            int intErrorCounter = 0;
            SqlConnection sqlCnn = new SqlConnection();
            try
            {
                string strProductPLU = string.Empty;
                string strProductName1 = string.Empty;
                string strProductName2 = string.Empty;
                string strProductDescription1 = string.Empty;
                string strProductDescription2 = string.Empty;
                string strProductBrand = string.Empty;
                string strProductBarcode = string.Empty;
                string strProductSKU = string.Empty;
                string strProductStatus = string.Empty;
                string strProductOriginCountry = string.Empty;
                string strProductRemarks = string.Empty;
                string strProductAttachIndex = string.Empty;
                string strProductUnit = string.Empty;
                string strProductWeigh = string.Empty;
                int intProductPackL = 0;
                int intProductPackM = 0;
                int intProductPackS = 0;
                string strProductSize = string.Empty;
                string strProductBoxSize = string.Empty;
                decimal decProductPackageCapacity = 0;
                decimal decProductNetWeight = 0;
                string strProductAttribute1 = string.Empty;
                string strProductAttribute2 = string.Empty;
                string strProductCreater = string.Empty;
                string strProductCreateDateTime = string.Empty;
                string strProductUpdater = string.Empty;
                string strProductUpdateDateTime = string.Empty;

                string strCategoryID = string.Empty;

                Logger.AddToFile("Transfer pos porduct data to PKProduct.");

                if (dtProduct.Rows.Count <= 0)
                {
                    Logger.AddToFile("No data for pos porduct.");
                    return false;
                }

                Logger.AddToFile("Setup database connection for pos porduct data update!");
                sqlCnn.ConnectionString = connectionString;
                sqlCnn.Open();

                for (int rowIndex = 0; rowIndex < dtProduct.Rows.Count; rowIndex++)
                {
                    strProductPLU = dtProduct.Rows[rowIndex]["productitemid"].ToString();
                    strProductName1 = dtProduct.Rows[rowIndex]["productname"].ToString();
                    strProductName2 = dtProduct.Rows[rowIndex]["productotherlanguagedisplay"].ToString();
                    strProductDescription1 = dtProduct.Rows[rowIndex]["productotherlanguagedisplay"].ToString();
                    //strProductDescription2 = dtProduct.Rows[rowIndex][""].ToString();
                    strProductBrand = dtProduct.Rows[rowIndex]["productbrand"].ToString();
                    strProductBarcode = dtProduct.Rows[rowIndex]["plu"].ToString();
                    //strProductSKU = dtProduct.Rows[rowIndex][""].ToString();

                    //TODO remove hard coded strings
                    switch (dtProduct.Rows[rowIndex]["status"].ToString())
                    {
                        case "available":
                            strProductStatus = "Active";
                            break;
                        case "pending":
                            strProductStatus = "Inactive";
                            break;
                        default:
                            strProductStatus = "Inactive";
                            break;
                    }

                    strProductOriginCountry = dtProduct.Rows[rowIndex]["productsupplier"].ToString();
                    strProductRemarks = dtProduct.Rows[rowIndex]["productremarks"].ToString();
                    //strProductAttachIndex = dtProduct.Rows[rowIndex][""].ToString();
                    strProductUnit = dtProduct.Rows[rowIndex]["productunit"].ToString();
                    switch (dtProduct.Rows[rowIndex]["productunit"].ToString())
                    {
                        case "ea":
                            strProductWeigh = "N";
                            break;
                        case "lb":
                            strProductWeigh = "Y";
                            break;
                        default:
                            strProductWeigh = "N";
                            break;
                    }
                    //strProductWeigh = dtProduct.Rows[rowIndex][""].ToString();
                    //intProductPackL = 0;
                    //intProductPackM = 0;
                    //intProductPackS = 0;
                    strProductSize = dtProduct.Rows[rowIndex]["productsize"].ToString();
                    strProductBoxSize = dtProduct.Rows[rowIndex]["productdimension"].ToString();
                    decimal.TryParse(dtProduct.Rows[rowIndex]["productqtyinbox"].ToString(), out decProductPackageCapacity);
                    //decimal.TryParse(dtProduct.Rows[rowIndex][""].ToString(), out decProductNetWeight);

                    //use attribute1 to save product sequence (the sequence in the category)
                    strProductAttribute1 = CommonFunctions.changeLengthto8byAdding0(dtProduct.Rows[rowIndex]["productsequence"].ToString());

                    //strProductAttribute2 = dtProduct.Rows[rowIndex][""].ToString();
                    strProductCreater = dtProduct.Rows[rowIndex]["createdby"].ToString();
                    DateTime tempDatetime;
                    DateTime.TryParse(dtProduct.Rows[rowIndex]["createdtime"].ToString(), out tempDatetime);
                    strProductCreateDateTime = tempDatetime.ToString(PKCommon.GlobalConst.strTimeStampFormat);
                    strProductUpdater = dtProduct.Rows[rowIndex]["updatedby"].ToString();
                    DateTime.TryParse(dtProduct.Rows[rowIndex]["updatedtime"].ToString(), out tempDatetime);
                    strProductUpdateDateTime = tempDatetime.ToString(PKCommon.GlobalConst.strTimeStampFormat);

                    string strCategoryNmae = dtProduct.Rows[rowIndex]["productupper"].ToString();
                    using (SqlCommand cmdCategoryID = new SqlCommand())
                    {
                        cmdCategoryID.CommandText = "SELECT ID FROM PKCategory WHERE (Name = '" + strCategoryNmae + "')";
                        cmdCategoryID.Connection = sqlCnn;
                        SqlDataReader drCategoryID = cmdCategoryID.ExecuteReader();
                        while (drCategoryID.Read())
                        {
                            strCategoryID = drCategoryID[0].ToString();
                        }
                        drCategoryID.Close();
                    }

                    //Logger.AddToFile("Setup parameters for pos product data update.");

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlCnn;

                    //check exist
                    SqlCommand cmdDistinct = new SqlCommand();
                    cmdDistinct.Connection = sqlCnn;
                    cmdDistinct.CommandText = "select [PLU] from PKProduct where [PLU]=@PLU";
                    cmdDistinct.Parameters.AddWithValue("@PLU", strProductPLU);
                    SqlDataReader dr = cmdDistinct.ExecuteReader();
                    if (dr.HasRows)
                    {
                        intUpdateCounter++;
                        //Logger.AddToFile("Update ProductPLU=" + strProductPLU);
                        cmd.CommandText = "update PKProduct set CategoryID=@CategoryID, Name1=@Name1, Name2=@Name2, Description1=@Description1, Description2=@Description2, Brand=@Brand, Barcode=@Barcode, SKU=@SKU, Status=@Status, OriginCountry=@OriginCountry, Remarks=@Remarks, AttachIndex=@AttachIndex, Unit=@Unit, Weigh=@Weigh, PackL=@PackL, PackM=@PackM, PackS=@PackS, Size=@Size, BoxSize=@BoxSize, PackageCapacity=@PackageCapacity, NetWeight=@NetWeight, Attribute1=@Attribute1, Attribute2=@Attribute2, Creater=@Creater, CreateDateTime=@CreateDateTime, Updater=@Updater, UpdateDateTime=@UpdateDateTime  where PLU=@PLU";
                    }
                    else
                    {
                        intInsertCounter++;
                        //Logger.AddToFile("Insert ProductPLU=" + strProductPLU);
                        cmd.CommandText = "insert into PKProduct (ID, CategoryID, PLU, Name1, Name2, Description1, Description2, Brand, Barcode, SKU, Status, OriginCountry, Remarks, AttachIndex, Unit, Weigh, PackL, PackM, PackS, Size, BoxSize, PackageCapacity, NetWeight, Attribute1, Attribute2, Creater, CreateDateTime, Updater, UpdateDateTime) values (@ID, @CategoryID, @PLU, @Name1, @Name2, @Description1, @Description2, @Brand, @Barcode, @SKU, @Status, @OriginCountry, @Remarks, @AttachIndex, @Unit, @Weigh, @PackL, @PackM, @PackS, @Size, @BoxSize, @PackageCapacity, @NetWeight, @Attribute1, @Attribute2, @Creater, @CreateDateTime, @Updater, @UpdateDateTime)";
                        cmd.Parameters.AddWithValue("@ID", Guid.NewGuid().ToString());
                    }
                    cmdDistinct.Dispose();
                    dr.Close();

                    cmd.Parameters.AddWithValue("@CategoryID", strCategoryID);
                    cmd.Parameters.AddWithValue("@PLU", strProductPLU);
                    cmd.Parameters.AddWithValue("@Name1", strProductName1);
                    cmd.Parameters.AddWithValue("@Name2", strProductName2);
                    cmd.Parameters.AddWithValue("@Description1", strProductDescription1);
                    cmd.Parameters.AddWithValue("@Description2", strProductDescription2);
                    cmd.Parameters.AddWithValue("@Brand", strProductBrand);
                    cmd.Parameters.AddWithValue("@Barcode", strProductBarcode);
                    cmd.Parameters.AddWithValue("@SKU", strProductSKU);
                    cmd.Parameters.AddWithValue("@Status", strProductStatus);
                    cmd.Parameters.AddWithValue("@OriginCountry", strProductOriginCountry);
                    cmd.Parameters.AddWithValue("@Remarks", strProductRemarks);
                    cmd.Parameters.AddWithValue("@AttachIndex", strProductAttachIndex);
                    cmd.Parameters.AddWithValue("@Unit", strProductUnit);
                    cmd.Parameters.AddWithValue("@Weigh", strProductWeigh);
                    cmd.Parameters.AddWithValue("@PackL", intProductPackL);
                    cmd.Parameters.AddWithValue("@PackM", intProductPackM);
                    cmd.Parameters.AddWithValue("@PackS", intProductPackS);
                    cmd.Parameters.AddWithValue("@Size", strProductSize);
                    cmd.Parameters.AddWithValue("@BoxSize", strProductBoxSize);
                    cmd.Parameters.AddWithValue("@PackageCapacity", decProductPackageCapacity);
                    cmd.Parameters.AddWithValue("@NetWeight", decProductNetWeight);
                    cmd.Parameters.AddWithValue("@Attribute1", strProductAttribute1);
                    cmd.Parameters.AddWithValue("@Attribute2", strProductAttribute2);
                    cmd.Parameters.AddWithValue("@Creater", strProductCreater);
                    cmd.Parameters.AddWithValue("@CreateDateTime", strProductCreateDateTime);
                    cmd.Parameters.AddWithValue("@Updater", strProductUpdater);
                    cmd.Parameters.AddWithValue("@UpdateDateTime", strProductUpdateDateTime);

                    //Logger.AddToFile("Begin execute.");

                    int intRows = cmd.ExecuteNonQuery();
                    if (intRows > 0)
                    {
                        isUpdated = true;
                        //Logger.AddToFile("Updated ProductPLU=" + strProductPLU);
                    }
                    else
                    {
                        isUpdated = false;
                        //Logger.AddToFile("Update failed ProductPLU=" + strProductPLU);
                        intErrorCounter++;
                    }
                }
                sqlCnn.Close();
                return isUpdated;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in TransferPOSProductData:" + ex.Message);
                isUpdated = false;
                return isUpdated;
            }
            finally
            {
                if (sqlCnn.State == ConnectionState.Open)
                {
                    sqlCnn.Close();
                }
                Logger.AddToFile(string.Format("The intErrorCounter={0}", intErrorCounter));
                Logger.AddToFile(string.Format("The intInsertCounter={0}", intInsertCounter));
                Logger.AddToFile(string.Format("The intUpdateCounter={0}", intUpdateCounter));

            }
        }

        private DataTable GetPOSModuleData(string strModule, out string strErr)
        {
            string commandText = string.Empty; ;
            //string strStream = string.Empty;
            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection();
            try
            {
                commandText = "SELECT uid, productname, productdisplay, plu, productunit, productupper, productsequence, productfastsequence, productdiscountable, status, isopensale, productinstock, productfrequency, producttype, storeid, mixandmatchid, productminstock, productmaxstock, productremarks, createdby, createdtime, updatedby, updatedtime, producttearminus, productotherlanguagedisplay, productbuttoncolor, productwarranty, productsupplier, productcost, productmarkup, productpicture, productzone, productinvcontrol, productbrand, productsize, productitemid, productdimension, productqtyinbox, productcateid, productstocknumber, productcommrate, productispackage, productpointsrate FROM producttemp WHERE ";
                switch (strModule)
                {
                    case POSModuleNames.POSDepartment:
                        commandText += " (producttype = 'department') ";
                        break;
                    case POSModuleNames.POSCategory:
                        commandText += " (producttype = 'group') ";
                        break;
                    case POSModuleNames.POSProduct:
                        commandText += " (producttype = 'plu') ";
                        break;
                    default:
                        break;
                }
                commandText += " AND (productitemid <> '')";
                //strStream = PKCommon.CommonFunctions.Read2Stream(connectionString, commandText, out strErr);
                cn.ConnectionString = connectionString;
                SqlDataAdapter da = new SqlDataAdapter(commandText, cn);
                cn.Open();
                da.Fill(dt);
                cn.Close();
                strErr = string.Empty;
                return dt;
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                return dt;
            }
            finally
            {
                cn.Close();
            }
        }

    }

    public struct POSModuleNames
    {
        public const string POSDepartment = "POSDepartment";
        public const string POSCategory = "POSCategory";
        public const string POSProduct = "POSProduct";
    }
}
