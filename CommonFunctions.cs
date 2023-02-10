using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Web.Configuration;
using System.Net.NetworkInformation;
//----------------------------------------
using System.ServiceProcess;  
using System.Diagnostics;
using System.Collections;

namespace PKCommon
{
    public class CommonFunctions
    {
        public static string Read2Stream(string connectionString, string strSelectCommandText, out string strErr)
        {
            SqlConnection sqlCnn = new SqlConnection();
            sqlCnn.ConnectionString = connectionString;
            strErr = string.Empty;
            try
            {
                sqlCnn.Open();

                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlCnn;
                sqlCmd.CommandText = strSelectCommandText;
                Logger.AddToFile("strSelectCommandText: " + strSelectCommandText);
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = sqlCmd;

                DataSet ds = new DataSet();
                sqlDA.Fill(ds);

                ////string strRootDir = HttpContext.Current.Request.PhysicalApplicationPath;
                //string strRootDir = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                //Guid g = Guid.NewGuid();
                //string strPath = strRootDir + g.ToString() + @"tempdata.xml";
                ////string strPath = System.Environment.CurrentDirectory + @"\tempdata.xml";
                //ds.WriteXml(strPath, XmlWriteMode.WriteSchema);
                //XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(strPath);
                //string bufferStream = xmlDoc.InnerXml;
                //File.Delete(strPath);

                string bufferStream = DataSet2XMLDataStream(ds); //xmlDoc.InnerXml;

                return bufferStream;
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
                return string.Empty;
            }
            finally
            {
                sqlCnn.Close();
            }
        }

        public static string DataSet2XMLDataStream(DataSet ds)
        {
            // This is the final document
            //XmlDocument xmlDoc = new XmlDocument();
            // Create a string writer that will write the Xml to a string
            StringWriter stringWriter = new StringWriter();
            // The Xml Text writer acts as a bridge between the xml stream and the text stream
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            // Now take the Dataset and extract the Xml from it, it will write to the string writer
            ds.WriteXml(xmlTextWriter, XmlWriteMode.WriteSchema);
            // Write the Xml out to a string
            //string contentAsXmlString = stringWriter.ToString();
            // load the string of Xml into the document
            //xmlDoc.LoadXml(contentAsXmlString);
            return stringWriter.ToString();
        }

        public static DataSet XMLDataStream2DataSet(string strDataStream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strDataStream);
            XmlNodeReader xmlReader = new XmlNodeReader(xmlDoc);
            DataSet ds = new DataSet();
            ds.ReadXml(xmlReader);
            return ds;
        }

        public static string BackAnHour(string strTime)
        {
            return ChangeTimeByHour(strTime, -1);
        }

        public static string AddAnHour(string strTime)
        {
            return ChangeTimeByHour(strTime, 1);
        }

        private static string ChangeTimeByHour(string strTime, int intAdjustment)
        {
            try
            {
                string strOutTime = string.Empty;
                DateTime dt;
                DateTime.TryParse(strTime, out dt);
                dt = dt.AddHours(intAdjustment);
                strOutTime = dt.ToString(PKCommon.GlobalConst.strTimeStampFormat);
                Logger.AddToFile("BackAnHour: From " + strTime + " to " + strOutTime);
                return strOutTime;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in BackAnHour: " + ex.Message);
                return strTime;
            }
        }

        public static string changeLengthto8byAdding0(string strShortString)
        {
            return changeLengthbyAdding0(strShortString, 8);
        }

        public static string changeLengthbyAdding0(string strShortString, int intLength)
        {
            if (strShortString.Length >= intLength)
            {
                return strShortString;
            }
            string strLongString = string.Empty;
            int intShortStringLength = strShortString.Length;
            strLongString = strShortString;
            for (int i = intShortStringLength; i < intLength; i++)
            {
                strLongString = "0" + strLongString;
            }
            return strLongString;

        }

        public static string PortectConnectionString(string connectionStringName)
        {
            try
            {
                //string strRootDir = HttpContext.Current.Request.PhysicalApplicationPath;
                Configuration config = WebConfigurationManager.OpenWebConfiguration(null);
                ConfigurationSection connectionSection = config.GetSection("connectionStrings");
                if (connectionSection.SectionInformation.IsProtected == false)
                {
                    connectionSection.SectionInformation.ProtectSection("RSAProtectedConfigurationProvider");

                }
                string connectionString = WebConfigurationManager.ConnectionStrings[connectionStringName].ToString();
                return connectionString;
            }
            catch (Exception ex)
            {
                //TODO 
                string a = ex.Message;
                return string.Empty;
            }
        }

        public static bool SynchronizeGeneralData(string strConnectionString, DataTable generalDataTable, string tableName, string[] primaryKeys)
        {
            return SynchronizeGeneralData(strConnectionString, generalDataTable, tableName, primaryKeys, false);
        }

        /// <summary>
        /// update the datatable by the primary keys. the table should be the exactly same structure as datatable
        /// 1. check exists
        /// 2. if exists update, else insert
        /// </summary>
        /// <param name="generalDataTable"></param>
        /// <param name="tableName"></param>
        /// <param name="primaryKeys">a list of primary keys of the table</param>
        /// <param name="isInsertOnly">true: skip the update operation if the record exists</param>
        /// <returns></returns>
        public static bool SynchronizeGeneralData(string strConnectionString, DataTable generalDataTable, string tableName, string[] primaryKeys, bool isInsertOnly)
        {

            //string strConnectionString = CommonFunctions.PortectConnectionString("CentralConnectionString");
            SqlConnection cnSelect = new SqlConnection();
            cnSelect.ConnectionString = strConnectionString;

            SqlConnection cn = new SqlConnection();
            //SqlTransaction trans;

            cn.ConnectionString = strConnectionString;
            cn.Open();
            //Logger.AddToFile("pricecn.beginTransaction");
            //trans = pricecn.BeginTransaction();

            bool isExist = false;//identfy the opertion: true->update, false->insert

            int intProcessTotal = 0;
            int intExists = 0;
            int intInserted = 0;

            int errorCounter = 0;
            try
            {
                if (primaryKeys.Length == 0)
                {
                    Logger.AddToFile("No primary key, can't Synchronize data");
                    return false;
                }
                for (int i = 0; i < generalDataTable.Rows.Count; i++)
                {
                    //Logger.AddToFile("i=" + i);
                    intProcessTotal++;
                    //identify if data exists
                    string[] keyValues = new string[primaryKeys.Length];
                    string[] keyParameters = new string[primaryKeys.Length];
                    string strWhere = string.Empty;
                    for (int j = 0; j < primaryKeys.Length; j++)
                    {
                        keyValues[j] = generalDataTable.Rows[i][primaryKeys[j]].ToString();
                        keyParameters[j] = "@" + primaryKeys[j];
                    }
                    //Logger.AddToFile("productName=" + strProductName);
                    using (SqlCommand cmdSelect = new SqlCommand())
                    {
                        cmdSelect.Connection = cnSelect;
                        cmdSelect.CommandText = "select ";
                        cmdSelect.CommandText += primaryKeys[0];
                        cmdSelect.CommandText += " from ";
                        cmdSelect.CommandText += tableName;

                        for (int j = 0; j < primaryKeys.Length; j++)
                        {
                            if (j == 0)
                            {
                                strWhere += " where ";
                            }
                            else
                            {
                                strWhere += " and ";
                            }
                            strWhere += primaryKeys[j];
                            strWhere += "=";
                            strWhere += keyParameters[j];
                            strWhere += " ";

                            cmdSelect.Parameters.AddWithValue(keyParameters[j], keyValues[j]);
                        }
                        cmdSelect.CommandText += strWhere;
                        //Logger.AddToFile("To open cnSelect");
                        cnSelect.Open();
                        //Logger.AddToFile("To execute reader");
                        SqlDataReader dr = cmdSelect.ExecuteReader();
                        isExist = dr.HasRows;
                        //Logger.AddToFile("isExist = " + isExist);
                        cnSelect.Close();
                        //Logger.AddToFile("cn closed");
                    };
                    if (isExist)
                    {
                        if (!isInsertOnly)
                        {
                            using (SqlCommand cmdExecute = new SqlCommand())
                            {
                                cmdExecute.Connection = cn;
                                string strSet = string.Empty;
                                if (generalDataTable.Columns.Count > 0)
                                {
                                    foreach (DataColumn dc in generalDataTable.Columns)
                                    {
                                        strSet += dc.ColumnName;
                                        strSet += "=";
                                        strSet += "@";
                                        strSet += dc.ColumnName;
                                        strSet += ", ";
                                        cmdExecute.Parameters.AddWithValue("@" + dc.ColumnName, generalDataTable.Rows[i][dc]);
                                    }
                                    cmdExecute.CommandText = "UPDATE ";
                                    cmdExecute.CommandText += tableName;
                                    cmdExecute.CommandText += " SET ";
                                    cmdExecute.CommandText += strSet.Substring(0, strSet.Length - 2);
                                    cmdExecute.CommandText += strWhere;

                                    //Logger.AddToFile("parameters added");
                                    int affectedRows = cmdExecute.ExecuteNonQuery();
                                    //Logger.AddToFile("affectedrows=" + affectedRows);
                                    if (affectedRows <= 0)
                                    {
                                        errorCounter++;
                                    }
                                }
                            };
                        }
                        intExists++;
                    }
                    else
                    {
                        using (SqlCommand cmdExecute = new SqlCommand())
                        {
                            cmdExecute.Connection = cn;
                            string strColumns = string.Empty;
                            string strParameters = string.Empty;
                            if (generalDataTable.Columns.Count > 0)
                            {
                                foreach (DataColumn dc in generalDataTable.Columns)
                                {
                                    strColumns += dc.ColumnName;
                                    strColumns += ", ";
                                    strParameters += "@";
                                    strParameters += dc.ColumnName;
                                    strParameters += ", ";
                                    cmdExecute.Parameters.AddWithValue("@" + dc.ColumnName, generalDataTable.Rows[i][dc]);
                                }
                                cmdExecute.CommandText = "INSERT INTO ";
                                cmdExecute.CommandText += tableName;
                                cmdExecute.CommandText += " (";
                                cmdExecute.CommandText += strColumns.Substring(0, strColumns.Length - 2);
                                cmdExecute.CommandText += ") VALUES (";
                                cmdExecute.CommandText += strParameters.Substring(0, strParameters.Length - 2);
                                cmdExecute.CommandText += ")";

                                //Logger.AddToFile("parameters added");
                                int affectedRows = cmdExecute.ExecuteNonQuery();
                                //Logger.AddToFile("affectedrows=" + affectedRows);
                                if (affectedRows <= 0)
                                {
                                    errorCounter++;
                                }
                            }
                        };
                        intInserted++;
                    }
                }

                Logger.AddToFile(string.Format("The intProcessTotal={0}", intProcessTotal));
                Logger.AddToFile(string.Format("The intExists={0}", intExists));
                Logger.AddToFile(string.Format("The intInserted={0}", intInserted));
                Logger.AddToFile(string.Format("The errorCounter={0}", errorCounter));

                cn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddToFile("Error in SynchronizeGeneralData: " + tableName + " " + ex.Message);

                return false;
            }
            finally
            {
                if (cnSelect.State == ConnectionState.Open)
                {
                    cnSelect.Close();
                }
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
        }
        //public static bool GetCommonTable(string Sql, out string DataBuf, out string ErrorMsg)
        //{
        //    DataBuf = string.Empty;
        //    ErrorMsg = string.Empty;
        //    try
        //    {
        //        string DBConnectionString = WebConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
        //        DataTable dt = PKCommon.CommonFunctions.GetDataTable(Sql, DBConnectionString);
        //        StringBuilder Buf = new StringBuilder(string.Empty);
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            if (i > 0) Buf.Append("^@$");
        //            for (int j = 0; j < dt.Columns.Count; j++)
        //            {
        //                if (j > 0) Buf.Append(PKCommon.GlobalConst.strSeparator);
        //                if (dt.Columns[j].DataType == System.Type.GetType("System.DateTime"))
        //                    Buf.Append(dt.Columns[j].ColumnName + "=" + ((DateTime)dt.Rows[i][dt.Columns[j].ColumnName]).ToString(PKCommon.GlobalConst.strTimeStampFormat));
        //                else
        //                    Buf.Append(dt.Columns[j].ColumnName + "=" + dt.Rows[i][dt.Columns[j].ColumnName].ToString());
        //            }
        //        }
        //        DataBuf = Buf.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMsg = "GetCommonTable " + Sql + " Error: " + ex.Message;
        //        return false;
        //    }
        //    return true;

        //}
        public static bool GetCommonTable(string Sql, string DBConnectionString, string ImageField, string ImagePath, out string DataBuf, out string ErrorMsg)
        {
            SqlConnection cn = new SqlConnection(); 
            try
            {
                cn.ConnectionString = DBConnectionString;
                cn.Open();
                return GetCommonTable(Sql, cn, null, ImageField, ImagePath, out DataBuf, out ErrorMsg);
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }
        public static bool GetCommonTable(string Sql, SqlConnection cn, SqlTransaction transaction, string ImageField, string ImagePath, out string DataBuf, out string ErrorMsg)
        {
            DataBuf = string.Empty;
            ErrorMsg = string.Empty;
            int i, j;
            try
            {
                //string DBConnectionString = WebConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
                DataTable dt = PKCommon.CommonFunctions.GetDataTable(Sql, cn, transaction);
                StringBuilder Buf = new StringBuilder(string.Empty);
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    if (i > 0) Buf.Append(PKCommon.GlobalConst.LineSeparator);
                    for (j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j > 0) Buf.Append(PKCommon.GlobalConst.ItemSeparator);

                        if (dt.Columns[j].DataType == System.Type.GetType("System.DateTime"))
                        {
                            if (dt.Rows[i][dt.Columns[j].ColumnName] != DBNull.Value)
                                Buf.Append(dt.Columns[j].ColumnName + PKCommon.GlobalConst.EquSeparator + ((DateTime)dt.Rows[i][dt.Columns[j].ColumnName]).ToString(PKCommon.GlobalConst.strTimeFormat));
                            else
                                Buf.Append(dt.Columns[j].ColumnName + PKCommon.GlobalConst.EquSeparator + DateTime.Now.ToString(PKCommon.GlobalConst.strTimeFormat));
                        }
                        else if (dt.Columns[j].DataType == System.Type.GetType("System.String"))
                        {
                            if (string.IsNullOrEmpty(dt.Rows[i][dt.Columns[j].ColumnName].ToString()))
                                Buf.Append(dt.Columns[j].ColumnName + PKCommon.GlobalConst.EquSeparator + PKCommon.GlobalConst.EmptyStr);
                            else
                                Buf.Append(dt.Columns[j].ColumnName + PKCommon.GlobalConst.EquSeparator + dt.Rows[i][dt.Columns[j].ColumnName].ToString());
                        }
                        else if (dt.Columns[j].DataType == System.Type.GetType("System.Decimal"))
                        {
                            if (string.IsNullOrEmpty(dt.Rows[i][dt.Columns[j].ColumnName].ToString()))
                                Buf.Append(dt.Columns[j].ColumnName + PKCommon.GlobalConst.EquSeparator + "0");
                            else
                                Buf.Append(dt.Columns[j].ColumnName + PKCommon.GlobalConst.EquSeparator + dt.Rows[i][dt.Columns[j].ColumnName].ToString());
                        }
                        else
                        {
                            Buf.Append(dt.Columns[j].ColumnName + PKCommon.GlobalConst.EquSeparator + dt.Rows[i][dt.Columns[j].ColumnName].ToString());
                        }
                        if (dt.Columns[j].ColumnName.ToUpper() == ImageField.ToUpper())
                        {
                            try
                            {
                                //string FileName = ImagePath + "\\" + dt.Rows[i][dt.Columns[j].ColumnName].ToString() + ".jpg";
                                string FileName = ImagePath + "\\" + dt.Rows[i][dt.Columns[j].ColumnName].ToString();
                                PKCommon.Logger.AddToFile("file name: " + FileName);
                                if (File.Exists(FileName))
                                {
                                    PKCommon.Logger.AddToFile("found: " + FileName);
                                    string img = PKCommon.CommonFunctions.ConvertFileToBase64(FileName);
                                    Buf.Append(PKCommon.GlobalConst.ItemSeparator + "PKOrderIMG" + PKCommon.GlobalConst.EquSeparator +
                                        dt.Rows[i][dt.Columns[j].ColumnName].ToString() + ";" + img);
                                }
                            }
                            catch (Exception ex)
                            {
                                PKCommon.Logger.AddToFile(ImagePath + "\\" + dt.Rows[i][dt.Columns[j].ColumnName].ToString() + ".jpg:" + ex.Message);
                            }
                        }
                    }
                }
                DataBuf = Buf.ToString();
            }
            catch (Exception ex)
            {
                ErrorMsg = "GetCommonTable " + Sql + " Error: " + ex.Message;
                throw (new Exception(ErrorMsg));
            }
            return true;

        }
        public static bool UpdateTable(string TableName, string Filter, string DataStream, string ImagePath, string DBConnectionString, string[] primaryKeys, string VendorID)
        {
            SqlConnection cn = new SqlConnection();
            try
            {
                cn.ConnectionString = DBConnectionString;
                cn.Open();
                return UpdateTable(TableName, Filter, DataStream, ImagePath, cn, null, primaryKeys, VendorID);
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }
        public static bool UpdateTable(string TableName, string Filter, string DataStream, string ImagePath, SqlConnection cn, SqlTransaction Transaction, string[] primaryKeys, string VendorID)
        {
            string Sql = string.Empty;
            try
            {
                string[] DataList = DataStream.Split(new string[] { PKCommon.GlobalConst.LineSeparator }, StringSplitOptions.None);
                for (int i = 0; i < DataList.Length; i++)
                {
                    string Fields = string.Empty;
                    string Values = string.Empty;
                    string Set = string.Empty;
                    string ID = string.Empty;
                    string Where = string.Empty;
                    bool FirstItem = true;
                    string[] ItemList = DataList[i].Split(new string[] { PKCommon.GlobalConst.ItemSeparator }, StringSplitOptions.None);
                    for (int j = 0; j < ItemList.Length; j++)
                    {
                        string[] Item = ItemList[j].Split(new string[] { PKCommon.GlobalConst.EquSeparator }, StringSplitOptions.None);
                        if (Item.Length != 2) continue;
                        if (Filter.IndexOf("[" + Item[0] + "]") < 0) continue;
                        if (string.IsNullOrEmpty(Item[1])) continue;
                        if (Item[1] == PKCommon.GlobalConst.EmptyStr) Item[1] = string.Empty;
                        Item[1] = Item[1].Replace("'", "''");

                        if (Item[0] == "PKOrderIMG")
                        {
                            try
                            {
                                if (Item[1] == string.Empty) continue;
                                string[] img = Item[1].Split(';');
                                PKCommon.CommonFunctions.ConvertBase64StrToFile(img[1], ImagePath + "\\" + img[0]);
                                PKCommon.Logger.AddToFile(ImagePath + "\\" + img[0]);
                                Item[0] = "ImageFile";
                                Item[1] = img[0];
                                continue;
                            }
                            catch (Exception ex)
                            {
                                PKCommon.Logger.AddToFile("PKOrderIMG" + Item[1] + ": " + ex.Message);
                                continue;
                            }
                        }
                        
                        for (int k = 0; k < primaryKeys.Length; k++)
                        {
                            if (Item[0] == primaryKeys[k])
                            {
                                if (!string.IsNullOrEmpty(Where)) Where += " and ";
                                Where += primaryKeys[k] + "='" + Item[1] + "' ";
                            }
                        }

                        if (FirstItem)
                        {
                            Fields += "(" + Item[0];
                            Values += "('" + Item[1] + "'";
                            Set += Item[0] + "='" + Item[1] + "'";
                        }
                        else
                        {
                            Fields += ", " + Item[0];
                            Values += ", '" + Item[1] + "'";
                            Set += "," + Item[0] + "='" + Item[1] + "'";
                        }
                        FirstItem = false;
                    }
                    if (string.IsNullOrEmpty(Fields)) continue;
                    if (!string.IsNullOrEmpty(VendorID))
                    {
                        Fields += ", VendorID";
                        Values += ", '" + VendorID + "' ";
                        Set += ", VendorID ='" + VendorID + "'";
                    }
                    Fields += ")";
                    Values += ")";
                    if (!string.IsNullOrEmpty(Where)) Where = " Where " + Where;

                    Sql = "Select * from " + TableName + " " + Where;
                    
                    DataTable dt = PKCommon.CommonFunctions.GetDataTable(Sql, cn, Transaction);
                    if (dt.Rows.Count > 0)
                    {
                        Sql = "Update " + TableName + " Set " + Set + Where;
                    }
                    else
                    {
                        Sql = "Insert into " + TableName + Fields + " Values" + Values;
                    }

                    PKCommon.CommonFunctions.ExecSql(Sql, cn, Transaction);
                }
            }
            catch (Exception ex)
            {
                throw (new Exception("Update table " + TableName + " fail: " + ex.Message));
            }

            return true;
        }
        public static bool DeleteGeneralData(string strConnectionString, DataTable deletedDataTable, string tableName)
        {



            return false;
        }

        //--Michael20121023: test the connection between server and POS
        public static bool IsConnected2Server(string strURL)
        {
            Uri url = new Uri(strURL);
            //Uri url = new Uri("http://pktest.dnsd.info");
            string host = url.Host;
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 500);
                if (reply.Status == IPStatus.Success)
                {
                    result = true;
                }
            }
            catch (Exception ex) 
            {
                PKCommon.Logger.AddToFile(ex.Message);
                return false;
            }
            return result;
        }

        //--Kevin  20130829: Detect if sql server is installed
        public static bool ExistSQL()
        {
            bool sqlFlag = false;
            ServiceController[] services =  //get system service set
                ServiceController.GetServices();
            for (int i = 0; i < services.Length; i++)
            {
                //Test it with sqlserver 2008 on Win7/win8
                if (services[i].DisplayName.ToString() == "SQL Server (MSSQLSERVER)")
                    sqlFlag = true;  //return true if sql server service is found
            }
            return sqlFlag;
            //throw new System.NotImplementedException();
        }

        //--Kevin  20130829: Detect if sql server is started
        public static bool DetectSQLStatus()
        {
            // Detecting mysqlserver service
            ServiceController sc = new ServiceController("SQL Server (MSSQLSERVER)");
            if (sc.Status == ServiceControllerStatus.Stopped)
            {
                return false;
            }
            else
                return true;
            //throw new System.NotImplementedException(); 
        }

        //--Kevin  20140519: Restore database function for ENT and POS
        public static bool DBRestore(string strDBRestoreFolder, string strConnectionString, int intENTorPOS)    //1 is for ENT and 2 is for POS
        {
            //strConnectionString = @"server=localhost; uid=sa; pwd=12345; database=PKRetailFEPOSv2";
            string strSql1 = string.Empty;

            if (intENTorPOS == 1)
            {
                strSql1 = "use master restore database PKRetail_Realfood from disk='" + strDBRestoreFolder + "'";
            }
            else
            {
                strSql1 = "use master restore database PKRetailFEPOSv2 from disk='" + strDBRestoreFolder + "'";
            }

            using (SqlConnection con = new SqlConnection(strConnectionString))
            {
                con.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand(strSql1, con);
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    //PKCommon.Logger.AddToFile("Successfully!");
                    return true;
                }
                catch (Exception ex)
                {
                    PKCommon.Logger.AddToFile(ex.Message);
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }


        }

        ///<summary>
        ///Database name is fixed in this script. Need to improve it later
        ///</summary>
        //--Kevin  20140212: Add DBBacku function
        public static bool DBBackupToDisc(string strDBBackupFolder, string strConnectionString) //, string DBBACUPHour, string DBBACUPMinute
        {
            string strCurrentTime;  //Get the current time as string
            strCurrentTime = DateTime.Now.Year.ToString() +
                            DateTime.Now.Month.ToString() +
                            DateTime.Now.Day.ToString() +
                            DateTime.Now.Hour.ToString() +
                            DateTime.Now.Minute.ToString() +
                            DateTime.Now.Second.ToString();

            using (SqlConnection con = new SqlConnection())   //Backup Database to C:\Database_Backup
            {
                //strDBBackupFolder = "D:\\Backup";

                try
                {
                    //cnn.ConnectionString = "Data Source=localhost;Initial Catalog=PKRetailFEPOSv2;Persist Security Info=True;User ID=sa;Password=12345";
                    con.ConnectionString = strConnectionString;
                }
                catch (Exception ex)
                {
                    PKCommon.Logger.AddToFile(ex.Message);
                    return false;
                }

                try
                {
                    con.Open();
                    SqlCommand com = new SqlCommand();
                    com.CommandText = "BACKUP DATABASE PKRetailFEPOSv2 TO DISK='" + strDBBackupFolder + "\\PKRetailFEPOSv2_" + strCurrentTime + ".bak'";
                    com.Connection = con;
                    com.ExecuteNonQuery();
                    //PKCommon.Logger.AddToFile("CommonFunctions.cs->DB_Backup_ToDisc:" + strDBBackupFolder + "\\PKRetailFEPOSv2_" + strCurrentTime + ".bak'");
                    con.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    PKCommon.Logger.AddToFile(ex.Message);
                    return false;
                }
            }
        }

        //--Kevin  20140504: Delete Database Back file before the specified days
        public static bool DBFileDelete(string strpath, int intBeforeDay)
        {
            int intCount = 0;
            ArrayList all = new ArrayList();
            DirectoryInfo DBPathBak = new DirectoryInfo(strpath);
            DateTime fileDT;

            foreach (FileInfo fi in DBPathBak.GetFiles())
            {
                //The length of every .bak is more than 8 (.bak)
                if (fi.Name.Substring(0, 8).Equals("PKRetail") && fi.Extension.Equals(".bak"))
                {
                    intCount++;
                    all.Add(fi.Name);
                    fileDT = File.GetCreationTime(strpath + fi.Name);

                    if (DateTime.Now.Subtract(fileDT).TotalDays > intBeforeDay)
                    {
                        //delete the first file which is found by GetFiles(). File name will be sorted by date. 
                        string filename = all[0].ToString();
                        File.Delete(DBPathBak + "\\" + filename);
                        //PKCommon.Logger.AddToFile(DBPathBak + "\\" + filename);
                        all.RemoveAt(0);
                        intCount--;
                    }
                }
            }
            return true;
        }

        //--Kevin  201401213: Get setting from POS SystemSetting
        public static string GetSettingValueFromPOS(string functionName, string strConnectionString)
        {
            string functionNameValue = string.Empty;
            try
            {
                SqlConnection cnn = new SqlConnection();
                cnn.ConnectionString = strConnectionString;

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"Select * from SystemSetting where (FunctionName=@FunctionName)";
                cmd.Parameters.Add("@Functionname", SqlDbType.VarChar, 50).Value = functionName;
                cmd.Connection = cnn;
                cnn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        functionNameValue =  dr["Value"].ToString();
                    }
                }
                cmd.ExecuteNonQuery();
                cnn.Close();
                return functionNameValue;
            }
            catch (Exception ex)
            {
                PKCommon.Logger.AddToFile(ex.Message);
                return functionNameValue;
            }

        }

        public static int ExecSql(string sqlStr, string ConnectionStr, List<SqlParameter> param = null)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConnectionStr;
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, cn);
                cmd.CommandType = CommandType.Text;
                if (param != null)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        cmd.Parameters.Add(param[i]);
                    }
                }
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (new Exception("Exec sql fail: " + sqlStr + ": " + ex.Message));
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
            }
        }

        public static int ExecSql(string sqlStr, SqlConnection cn, SqlTransaction Transaction, List<SqlParameter> param = null)
        {
            try
            {
                SqlCommand cmd;
                if (Transaction != null)
                    cmd = new SqlCommand(sqlStr, cn, Transaction);
                else
                    cmd = new SqlCommand(sqlStr, cn);

                cmd.CommandType = CommandType.Text;                
                if (param != null)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        cmd.Parameters.Add(param[i]);
                    }
                }
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (new Exception("Exec sql fail: " +sqlStr + ": " + ex.Message));
            }

        }

        public static DataTable GetDataTable(string sqlStr, string ConnectionStr, List<SqlParameter> param = null)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = ConnectionStr;
            DataSet ds = new DataSet();
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(sqlStr, cn);
                cmd.CommandType = CommandType.Text;
                if (param != null)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        cmd.Parameters.Add(param[i]);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw (new Exception("GetDataTable Error: " + sqlStr + ": " + ex.Message));
            }
            finally
            {
                if (cn.State == ConnectionState.Open) cn.Close();
                if (ds != null) ds.Dispose();
            }
        }
        public static DataTable GetDataTable(string sqlStr, SqlConnection cn, SqlTransaction Transaction, List<SqlParameter> param = null)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd;
                if (Transaction != null)
                    cmd = new SqlCommand(sqlStr, cn, Transaction);
                else
                    cmd = new SqlCommand(sqlStr, cn);
                cmd.CommandType = CommandType.Text;
                if (param != null)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        cmd.Parameters.Add(param[i]);
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw (new Exception("GetDataTable Error: " + sqlStr + ": " + ex.Message));
            }
            finally
            {
                if (ds != null) ds.Dispose();
            }
        }

        public static string ConvertFileToBase64(string FileName)
        {
            FileStream fs = File.Open(FileName, FileMode.Open);
            byte[] buf = new byte[fs.Length];
            fs.Read(buf, 0, (int)fs.Length);
            fs.Close();
            return Convert.ToBase64String(buf);
        }

        public static void ConvertBase64StrToFile(string Base64, string FileName)
        {
            byte[] buf = Convert.FromBase64String(Base64);
            if (File.Exists(FileName)) File.Delete(FileName);
            FileStream fs = File.Open(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            fs.Write(buf, 0, buf.Length);
            fs.Close();
        }
    }
}
